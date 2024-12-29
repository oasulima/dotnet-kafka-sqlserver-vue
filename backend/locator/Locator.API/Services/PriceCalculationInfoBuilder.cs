using Locator.API.Models.Internal;
using Locator.API.Services.Interfaces;
using Shared;
using Shared.Constants;
using Shared.Settings;

namespace Locator.API.Services;

public class PriceCalculationInfoBuilder : IPriceCalculationInfoBuilder
{
    public Outcome<PriceCalculationInfo> BuildPriceCalculationInfo(Quote quote, IReadOnlyDictionary<string, ProviderSetting?> providersSettings)
    {
        var errors = new List<LocatorError>();
        var internalInventoryPrices = new List<PriceInfoEx>();
        var regularResponses = new Dictionary<string, PriceInfoEx[]>();

        foreach (var (provider, providerResponse) in quote.ProviderQuoteResponses)
        {
            if (providerResponse.Status != ProviderQuoteResponse.StatusEnum.Ready)
            {
                continue;
            }

            var providerId = provider.ToUpperInvariant();

            if (providerId is Providers.SelfIds.InternalInventory)
            {
                var prices = BuildInternalInventoryPrices(providersSettings, providerResponse).Unwrap(errors);

                internalInventoryPrices.AddRange(prices);
            }
            else
            {
                var prices = BuildRegularProviderPrices(providersSettings, providerId, providerResponse).Unwrap(errors);

                regularResponses.Add(providerId, prices.ToArray());
            }
        }

        var info = new PriceCalculationInfo(
            QuoteId: quote.Id,
            AccountId: quote.AccountId,
            Symbol: quote.Symbol,
            RequestedQuantity: quote.RequestedQuantity,
            InternalInventorySupply: internalInventoryPrices,
            RegularSupply: regularResponses);

        return new(info, errors);
    }

    private static Outcome<IEnumerable<PriceInfoEx>> BuildRegularProviderPrices(
        IReadOnlyDictionary<string, ProviderSetting?> providersSettings,
        string providerId,
        ProviderQuoteResponse providerResponse)
    {
        if (providerResponse.Prices == null)
        {
            return new(Enumerable.Empty<PriceInfoEx>());
        }

        var provider = providersSettings.GetValueOrDefault(providerId);

        if (provider == null)
        {
            var error = new LocatorError(LocatorErrorKind.RegularProviderProviderNotFound, $"ProviderId: {providerId}");

            return new(Array.Empty<PriceInfoEx>(), error);
        }

        var prices = new List<PriceInfoEx>();

        foreach (var priceInfo in providerResponse.Prices)
        {
            prices.Add(PriceInfoEx.FromProviderAndPriceInfo(priceInfo, provider));
        }

        return new(prices);
    }

    private static Outcome<IEnumerable<PriceInfoEx>> BuildInternalInventoryPrices(IReadOnlyDictionary<string, ProviderSetting?> providersSettings, ProviderQuoteResponse providerResponse)
    {
        if (providerResponse.Prices == null)
        {
            return new(Enumerable.Empty<PriceInfoEx>());
        }

        var errors = new List<LocatorError>();
        var prices = new List<PriceInfoEx>();

        foreach (var priceInfo in providerResponse.Prices)
        {
            var providerId = priceInfo.Source;

            if (providerId == null)
            {
                errors.Add(new LocatorError(LocatorErrorKind.InternalInvSourceProviderNotProvided));

                continue;
            }

            var provider = providersSettings.GetValueOrDefault(providerId);

            if (provider == null)
            {
                errors.Add(new LocatorError(LocatorErrorKind.InternalInvSourceProviderNotFound, $"ProviderId: {providerId}"));

                continue;
            }

            prices.Add(PriceInfoEx.FromProviderAndPriceInfo(priceInfo, provider));
        }

        return new(prices, errors);
    }
}

