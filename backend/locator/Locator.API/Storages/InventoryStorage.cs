using System.Data;
using LinqToDB;
using Locator.API.Data;
using Locator.API.Data.Entities;
using Locator.API.Storages.Interfaces;

namespace Locator.API.Storages;

public class InventoryStorage : IInventoryStorage
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public InventoryStorage(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public ICollection<AccountInventoryItemDb> GetInventory(
        string accountId,
        DateTime? afterDateTime = null
    )
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        var inventoryItems = linq2Db
            .AccountInventoryItems.Where(x => x.AccountId == accountId)
            .ToList();

        return GetRidOfOldVersions(inventoryItems);
    }

    public void SaveInventoryVersion(AccountInventoryItemDb inventoryItem)
    {
        SaveInventoryVersionInternal(inventoryItem, 1);
    }

    private void SaveInventoryVersionInternal(AccountInventoryItemDb inventoryItem, int attempt)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        linq2Db.Insert(inventoryItem);
    }

    public void DeleteAllInventories()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        linq2Db.AccountInventoryItems.Delete();
    }

    private static ICollection<AccountInventoryItemDb> GetRidOfOldVersions(
        List<AccountInventoryItemDb> inventoryItems
    )
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

        return result.Values;
    }
}
