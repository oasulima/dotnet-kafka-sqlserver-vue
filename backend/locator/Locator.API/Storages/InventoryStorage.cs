using System.Data;
using LinqToDB;
using Locator.API.Data;
using Locator.API.Data.Entities;
using Locator.API.Storages.Interfaces;

namespace Locator.API.Storages;

public class InventoryStorage : IInventoryStorage
{

    private readonly DbConnection linq2Db;

    public InventoryStorage(DbConnection linq2Db)
    {
        this.linq2Db = linq2Db;
    }

    public AccountInventoryItemDb[] GetInventory(string accountId, DateTime? afterDateTime = null)
    {
        var inventoryItems = linq2Db.AccountInventoryItems.Where(x => x.AccountId == accountId).ToList();


        return GetRidOfOldVersions(inventoryItems);

    }

    public void SaveInventoryVersion(AccountInventoryItemDb inventoryItem)
    {
        SaveInventoryVersionInternal(inventoryItem, 1);
    }

    private void SaveInventoryVersionInternal(AccountInventoryItemDb inventoryItem, int attempt)
    {
        linq2Db.Insert(inventoryItem);
    }

    public void DeleteAllInventories()
    {
        linq2Db.AccountInventoryItems.Delete();
    }

    private static AccountInventoryItemDb[] GetRidOfOldVersions(List<AccountInventoryItemDb> inventoryItems)
    {
        var result = new Dictionary<string, AccountInventoryItemDb>();

        foreach (var dbItem in inventoryItems)
        {
            var key = dbItem.Id;
            if (!result.TryGetValue(key, out var item) || item.Version < dbItem.Version)
            {
                result[key] = dbItem;
            }
        }

        return result.Values.ToArray();
    }
}