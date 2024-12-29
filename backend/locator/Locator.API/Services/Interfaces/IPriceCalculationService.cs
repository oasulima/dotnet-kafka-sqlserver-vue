using Locator.API.Models.Internal;
using Shared;
using static Locator.API.Services.PriceCalculationService;

namespace Locator.API.Services.Interfaces;

public interface IPriceCalculationService
{
    PriceCalculationResult CalculateAllProviderPrices(Quote quote);
    QuotePopulateResult PopulateProviderBuyOrderRequestsAndPrice(PriceCalculationResult priceCalculationResult, bool adminMode, decimal? maxPriceToAccept);
    decimal GetDiscountedPrice(PriceInfo priceInfo);
}

public record QuotePopulateResult(decimal Price, int QuantityToBuy,
    Dictionary<string, (string provider, ProviderBuyRequest request)> ProviderBuyOrderRequests);

public record PriceCalculationResult(PriceCalculationInfo Source,
    IReadOnlyCollection<ProviderOffer> Offers,
    IReadOnlyCollection<LocatorError> Errors);
