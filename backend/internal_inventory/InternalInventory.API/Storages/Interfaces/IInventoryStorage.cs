using InternalInventory.API.Data.Entities;

namespace InternalInventory.API.Storages.Interfaces;

public interface IInventoryStorage
{
    Dictionary<string, InternalInventoryItemDb> GetInventoryFromDb(DateTime? afterDateTime = null);
    void SaveCurrentVersion(InternalInventoryItemDb inventoryItem);
    void SaveInventoryItems(IList<InternalInventoryItemDb> inventoryItems);
    void DeleteAllInventoryItems();
}