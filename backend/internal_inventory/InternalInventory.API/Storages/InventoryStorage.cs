using System.Data;
using LinqToDB;
using LinqToDB.Data;
using InternalInventory.API.Data;
using InternalInventory.API.Data.Entities;
using InternalInventory.API.Storages.Interfaces;

namespace InternalInventory.API.Storages;

public class InventoryStorage : IInventoryStorage
{ 
    private readonly IServiceScopeFactory serviceScopeFactory;

    public InventoryStorage(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public Dictionary<string, InternalInventoryItemDb> GetInventoryFromDb(DateTime? afterDateTime = null)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        var inventoryItems = linq2Db.InternalInventoryItems.Where(x => afterDateTime == null || x.Timestamp > afterDateTime).ToList();

        return GetRidOfOldVersions(inventoryItems);
    }

    public void SaveCurrentVersion(InternalInventoryItemDb inventoryItem)
    {
        SaveCurrentVersionInternal(inventoryItem, 1);
    }

    private void SaveCurrentVersionInternal(InternalInventoryItemDb inventoryItem, int attempt = 1)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        linq2Db.Insert(inventoryItem);
    }

    public void SaveInventoryItems(IList<InternalInventoryItemDb> inventoryItems)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        linq2Db.BulkCopy(inventoryItems);
    }

    public void DeleteAllInventoryItems()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        linq2Db.InternalInventoryItems.Delete();
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