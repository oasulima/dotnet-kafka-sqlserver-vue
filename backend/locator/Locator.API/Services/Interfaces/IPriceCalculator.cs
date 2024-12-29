using Shared;
using Shared.Settings;

namespace Locator.API.Services.Interfaces;

public interface IPriceCalculator
{
    /// <summary>
    /// Returns null in case providers min price couldn't be determined, decimal price otherwise
    /// </summary>
    Outcome<decimal?> GetProvidersMinPrice(PriceCalculationInfo info);

    decimal GetDiscountedPrice(PriceInfo price, ProviderSetting? provider);
}
