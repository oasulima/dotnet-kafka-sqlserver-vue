using Shared;

namespace Locator.API.Services.Interfaces;

public interface ILocatorService
{
    void CreateQuote(QuoteRequest quoteRequest, DateTime timeRequestArrived);
    void ProcessProviderQuoteResponse(
        ProviderQuoteResponse providerQuoteResponse,
        string providerId
    );
    void AcceptQuote(QuoteRequest quote);
    void CancelQuote(QuoteRequest quote);
    void ProcessProviderBuyOrderResponse(
        ProviderBuyResponse providerBuyOrderResponse,
        string providerId
    );
    void ProcessHangingQuotes();
}
