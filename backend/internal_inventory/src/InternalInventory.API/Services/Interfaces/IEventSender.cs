using Shared;

namespace InternalInventory.API.Services.Interfaces;

public interface IEventSender
{
    void SendBuyResponseEvent(ProviderBuyResponse orderEvent);
    void SendQuoteResponseReadyEvent(ProviderQuoteResponse response);
    void SendInternalInventoryItemChangeEvent(InternalInventoryItem item);
}
