using Locator.API.Services.Interfaces;
using Shared;
using Shared.Settings;

namespace Locator.API.Services;

public class PriceCalculator : IPriceCalculator
{
    public Outcome<decimal?> GetProvidersMinPrice(PriceCalculationInfo info)
    {
        var externalProviderPrices = info.RegularSupply.Values.SelectMany(x => x).ToArray();

        var minProviderPrice = externalProviderPrices
            .Where(x => x.PriceInfo.Quantity > 0)
            .Min(x => (decimal?)GetDiscountedPrice(x.PriceInfo, x.ProviderSetting));

        if (minProviderPrice.HasValue)
        {
            return new(minProviderPrice.Value);
        }

        var warnLogMessage = "Cannot determine external providers current min price. " +
                             $"Quote {info.QuoteId}, Symbol {info.Symbol} " +
                             "Trying to get external providers latest available min price.";

        minProviderPrice = externalProviderPrices
            .Where(x => x.PriceInfo.Quantity == 0 && x.PriceInfo.Price > 0)
            .Min(x => (decimal?)GetDiscountedPrice(x.PriceInfo, x.ProviderSetting));

        if (minProviderPrice.HasValue)
        {
            return new(minProviderPrice.Value);
        }

        warnLogMessage = "Cannot determine external providers latest available min price. " +
                         $"Quote {info.QuoteId}, Symbol {info.Symbol} " +
                         "Trying to get min price from Internal Inventory.";

        minProviderPrice = info.InternalInventorySupply
            .Where(x => x.PriceInfo.Price > 0)
            .Min(x => (decimal?)GetDiscountedPrice(x.PriceInfo, x.ProviderSetting));

        if (minProviderPrice.HasValue)
        {
            return new(minProviderPrice.Value);
        }

        var errorLogMessage = "Cannot determine providers min price" +
                              $"Quote {info.QuoteId}, Symbol {info.Symbol}";

        return new(null);
    }

    public decimal GetDiscountedPrice(PriceInfo price, ProviderSetting? provider)
    {
        var discount = provider?.Discount ?? 0;

        return price.Price * (1 - discount);
    }
}