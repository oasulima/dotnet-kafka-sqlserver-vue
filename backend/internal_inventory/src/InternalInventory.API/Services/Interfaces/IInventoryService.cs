using Shared;

namespace InternalInventory.API.Services.Interfaces;

public interface IInventoryService
{
    IList<InternalInventoryItem> GetInventory(string symbol, string? providerId);
    InternalInventoryItem? UpdateInventory(InternalInventoryItem item);
    InternalInventoryItem? MakeInactive(string id);
    InternalInventoryItem? MakeActive(string id, int version);
    InternalInventoryItem? DeleteInventoryItem(string id, int version);
    void ProcessQuoteRequest(ProviderQuoteRequest quoteRequest);
    void ProcessBuyRequest(ProviderBuyRequest buyRequest);
    InternalInventoryItem AddInventory(AddInternalInventoryItemRequest message);
    void ClearCache();
}
