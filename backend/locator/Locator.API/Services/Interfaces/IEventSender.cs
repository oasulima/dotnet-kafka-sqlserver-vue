using Shared;

namespace Locator.API.Services.Interfaces;

public interface IEventSender
{
    void SendLocatorQuoteRequest(QuoteRequest quoteRequest);

    void SendProviderQuoteRequest(string topic, ProviderQuoteRequest providerQuoteRequest);
    void SendQuoteResponse(QuoteResponse quoteResponse);

    void SendProviderBuyOrderRequest(string providerConfigurationBuyRequestTopic,
        ProviderBuyRequest orderRequest);

    void SendAddInternalInventoryItemRequest(AddInternalInventoryItemRequest request);
    void SendInvalidateCacheCommand(SyncCommand invalidateCacheCommand);
}