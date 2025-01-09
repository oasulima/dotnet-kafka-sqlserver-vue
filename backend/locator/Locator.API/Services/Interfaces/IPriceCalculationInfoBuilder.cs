using Locator.API.Models.Internal;
using Shared;
using Shared.Settings;

namespace Locator.API.Services.Interfaces;

public interface IPriceCalculationInfoBuilder
{
    Outcome<PriceCalculationInfo> BuildPriceCalculationInfo(
        Quote quote,
        IReadOnlyDictionary<string, ProviderSetting?> providersSettings
    );
}

public record PriceCalculationInfo(
    string QuoteId,
    string AccountId,
    string Symbol,
    int RequestedQuantity,
    IReadOnlyList<PriceInfoEx> InternalInventorySupply,
    IReadOnlyDictionary<string, PriceInfoEx[]> RegularSupply
);

public record PriceInfoEx(
    PriceInfo PriceInfo,
    ProviderSetting ProviderSetting,
    decimal DiscountedPrice
)
{
    public static PriceInfoEx FromProviderAndPriceInfo(
        PriceInfo priceInfo,
        ProviderSetting providerSetting
    )
    {
        var discountedPrice = priceInfo.Price * (1 - providerSetting.Discount);

        return new PriceInfoEx(priceInfo, providerSetting, discountedPrice);
    }
}
