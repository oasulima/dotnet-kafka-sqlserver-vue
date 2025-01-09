using Locator.API.Models.Internal;
using Locator.API.Services.Interfaces;
using Locator.API.Storages.Interfaces;
using Locator.API.Utility;
using Shared;
using Shared.Constants;

namespace Locator.API.Services;

public class PriceCalculationService : IPriceCalculationService
{
    private readonly IProviderSettingStorage _providerSettingStorage;
    private readonly ISettingService _settingService;
    private readonly IPriceCalculationInfoBuilder _priceCalculationInfoBuilder;
    private readonly IPriceCalculator _priceCalculator;
    private readonly ILocatorErrorReporter _locatorErrorReporter;

    private const int QuotePriceRoundDecimals = 4;

    public PriceCalculationService(
        IProviderSettingStorage providerSettingStorage,
        ISettingService settingService,
        IPriceCalculationInfoBuilder priceCalculationInfoBuilder,
        IPriceCalculator priceCalculator,
        ILocatorErrorReporter locatorErrorReporter
    )
    {
        _providerSettingStorage = providerSettingStorage;
        _settingService = settingService;
        _priceCalculationInfoBuilder = priceCalculationInfoBuilder;
        _priceCalculator = priceCalculator;
        _locatorErrorReporter = locatorErrorReporter;
    }

    public decimal GetDiscountedPrice(PriceInfo priceInfo)
    {
        var settings = _providerSettingStorage.Get(priceInfo.Source);

        return _priceCalculator.GetDiscountedPrice(priceInfo, settings);
    }

    public PriceCalculationResult CalculateAllProviderPrices(Quote quote)
    {
        var commonDetails =
            $"AccountId: {quote.AccountId}, QuoteId: {quote.Id}, Symbol: {quote.Symbol}";
        var errors = new List<LocatorError>();

        var providerIds = quote
            .ProviderQuoteResponses.Keys.Concat(
                quote.ProviderQuoteResponses.SelectMany(x =>
                    (x.Value.Prices ?? Array.Empty<PriceInfo>()).Select(p => p.Source)
                )
            )
            .Distinct()
            .Select(x => x.ToUpperInvariant())
            .Distinct();

        var providersSettings = providerIds
            .Select(x => _providerSettingStorage.Get(x))
            .Where(x => x != null)
            .ToDictionary(x => x!.ProviderId);

        var info = _priceCalculationInfoBuilder
            .BuildPriceCalculationInfo(quote, providersSettings)
            .Unwrap(errors);

        var providersMinPrice = _priceCalculator.GetProvidersMinPrice(info).Unwrap(errors);

        var discountedPrices = CalculateDiscountedPrices(info, providersMinPrice).Unwrap(errors);

        var offers = new List<ProviderOffer>();
        offers.AddRange(discountedPrices);

        _locatorErrorReporter.Report(errors, nameof(CalculateAllProviderPrices), commonDetails);

        return new PriceCalculationResult(Source: info, Offers: offers, Errors: errors);
    }

    public QuotePopulateResult PopulateProviderBuyOrderRequestsAndPrice(
        PriceCalculationResult pricing,
        bool adminMode,
        decimal? maxPriceToAccept
    )
    {
        var providerDiscountedPrices = pricing.Offers;
        var totalCost = 0m;
        var totalQuantity = 0;
        var maxDiscountedPrice = 0m;
        var quantityToFind = pricing.Source.RequestedQuantity;
        var requests = new Dictionary<string, ProviderBuyRequest>();
        var prices = providerDiscountedPrices.AsEnumerable();

        if (maxPriceToAccept != null)
        {
            prices = prices.Where(x => x.Price <= maxPriceToAccept);
        }

        foreach (var discountedPrice in prices.OrderBy(x => x.DiscountedPrice))
        {
            var quantityFound = Math.Min(quantityToFind, discountedPrice.Quantity);
            if (quantityFound <= 0)
            {
                continue;
            }

            quantityToFind -= quantityFound;
            if (!requests.TryGetValue(discountedPrice.Provider, out var request))
            {
                request = pricing.Source.ToProviderBuyRequest();
            }

            request.RequestedAssets.Add(
                new PriceInfo
                {
                    Price = discountedPrice.Price,
                    Quantity = quantityFound,
                    Source = discountedPrice.Source,
                }
            );
            requests[discountedPrice.Provider] = request;
            var price = discountedPrice.Price;
            totalCost += price * quantityFound;
            totalQuantity += quantityFound;
            maxDiscountedPrice = Math.Max(maxDiscountedPrice, discountedPrice.DiscountedPrice);
        }

        var providerBuyOrderRequests =
            new Dictionary<string, (string provider, ProviderBuyRequest request)>();

        foreach (var (key, value) in requests)
        {
            providerBuyOrderRequests[value.Id] = (key, value);
        }

        if (totalQuantity == 0)
        {
            return new QuotePopulateResult(
                Price: 0,
                QuantityToBuy: totalQuantity,
                ProviderBuyOrderRequests: providerBuyOrderRequests
            );
        }

        var quotePrice = Math.Round(totalCost / totalQuantity, QuotePriceRoundDecimals);
        if (!adminMode)
        {
            quotePrice = Math.Max(quotePrice, maxDiscountedPrice);
        }

        return new QuotePopulateResult(
            Price: quotePrice,
            QuantityToBuy: totalQuantity,
            ProviderBuyOrderRequests: providerBuyOrderRequests
        );
    }

    private Outcome<IReadOnlyCollection<ProviderOffer>> CalculateDiscountedPrices(
        PriceCalculationInfo info,
        decimal? providersMinPrice
    )
    {
        var prices = new List<ProviderOffer>();
        var errors = new List<LocatorError>();

        var iiPrices = CalculateInternalInventoryDiscountedPrices(
                providersMinPrice,
                info.InternalInventorySupply
            )
            .Unwrap(errors);

        prices.AddRange(iiPrices);

        foreach (var (provider, providerResponse) in info.RegularSupply)
        {
            prices.AddRange(
                CalculateStandardProviderDiscountedPrices(provider, info.Symbol, providerResponse)
            );
        }

        return new(prices, errors);
    }

    private IEnumerable<ProviderOffer> CalculateStandardProviderDiscountedPrices(
        string provider,
        string symbol,
        IReadOnlyList<PriceInfoEx> priceAndProviderInfos
    )
    {
        foreach (var (priceInfo, providerSetting, discountedPrice) in priceAndProviderInfos)
        {
            yield return new ProviderOffer(
                Provider: provider,
                Source: priceInfo.Source,
                Price: priceInfo.Price,
                DiscountedPrice: discountedPrice,
                Quantity: priceInfo.Quantity
            );
        }
    }

    private Outcome<IEnumerable<ProviderOffer>> CalculateInternalInventoryDiscountedPrices(
        decimal? providersMinPrice,
        IReadOnlyList<PriceInfoEx> priceAndProviderInfos
    )
    {
        var prices = new List<ProviderOffer>();
        var errors = new List<LocatorError>();

        foreach (var (priceInfo, sourceProvider, discountedPrice) in priceAndProviderInfos)
        {
            prices.Add(
                new ProviderOffer(
                    Provider: Providers.SelfIds.InternalInventory,
                    Source: priceInfo.Source,
                    Price: priceInfo.Price,
                    DiscountedPrice: discountedPrice,
                    Quantity: priceInfo.Quantity
                )
            );
        }

        return new(prices, errors);
    }

    public record ProviderOffer(
        string Provider,
        string Source,
        decimal Price,
        decimal DiscountedPrice,
        int Quantity
    );
}
