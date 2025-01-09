using InternalInventory.API.Data.Entities;

namespace InternalInventory.API.Data.Repositories;

public interface IInternalInventoryItemRepository
{
    InternalInventoryItemDb[] Get(DateTime? afterDateTime = null);

    void Add(InternalInventoryItemDb inventoryItem);

    void Add(IList<InternalInventoryItemDb> inventoryItems);

    void DeleteAll();
}
