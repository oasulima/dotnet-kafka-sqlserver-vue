using Reporting.API.Data.Models.DbModels;
using Shared;

namespace Reporting.API.Data.Repositories.Interfaces;

public interface IInternalInventoryItemRepository
{
    InternalInventoryItemDb[] GetInternalInventoryItems(DateTime? from = null,
        DateTime? to = null, string? symbol = null, CreatingType? creatingType = null,
        InternalInventoryItem.State? status = null);
    InternalInventoryItemDb[] GetInternalInventoryItemsHistory(int take, string? providerId = null, string? symbol = null,
        DateTime? beforeCreatedAt = null); 
    void AddInternalInventoryItem(InternalInventoryItemDb inventoryItem);
}