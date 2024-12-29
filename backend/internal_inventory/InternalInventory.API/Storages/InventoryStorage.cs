using System.Data;
using LinqToDB;
using LinqToDB.Data;
using InternalInventory.API.Data;
using InternalInventory.API.Data.Entities;
using InternalInventory.API.Storages.Interfaces;

namespace InternalInventory.API.Storages;

public class InventoryStorage : IInventoryStorage
{

    private readonly DbConnection _linq2Db;

    public InventoryStorage(DbConnection linq2db)
    {
        _linq2Db = linq2db;
    }

    public Dictionary<string, InternalInventoryItemDb> GetInventoryFromDb(DateTime? afterDateTime = null)
    {
        var inventoryItems = _linq2Db.InternalInventoryItems.Where(x => afterDateTime == null || x.Timestamp > afterDateTime).ToList();

        return GetRidOfOldVersions(inventoryItems);
    }

    public void SaveCurrentVersion(InternalInventoryItemDb inventoryItem)
    {
        SaveCurrentVersionInternal(inventoryItem, 1);
    }

    private void SaveCurrentVersionInternal(InternalInventoryItemDb inventoryItem, int attempt = 1)
    {
        _linq2Db.Insert(inventoryItem);
    }

    public void SaveInventoryItems(IList<InternalInventoryItemDb> inventoryItems)
    {
        _linq2Db.BulkCopy(inventoryItems);
    }

    public void DeleteAllInventoryItems()
    {
        _linq2Db.InternalInventoryItems.Delete();
    }

    private static Dictionary<string, InternalInventoryItemDb> GetRidOfOldVersions(
        List<InternalInventoryItemDb> inventoryItems)
    {
        var result = new Dictionary<string, InternalInventoryItemDb>();

        foreach (var dbItem in inventoryItems)
        {
            if (!result.TryGetValue(dbItem.Id, out var item) || item.Version < dbItem.Version)
            {
                result[dbItem.Id] = dbItem;
            }
        }

        return result;
    }
}