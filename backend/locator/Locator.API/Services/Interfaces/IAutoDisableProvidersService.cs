namespace Locator.API.Services.Interfaces;

using TProviderId = System.String;
using TSymbol = System.String;

public interface IAutoDisableProvidersService
{
    void RegisterProviderQuote(string provider, string symbol, bool isAnswered);
    void RegisterProviderBuy(string provider, string symbol, bool isAnswered);
    bool IsProviderDisabled(string provider, string symbol);
    Dictionary<TProviderId, TSymbol[]?> GetDisabledProviders();
    void EnableProviderBack(string provider);
    void Clear();
}
