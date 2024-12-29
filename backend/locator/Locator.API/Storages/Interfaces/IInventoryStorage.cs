using Locator.API.Data.Entities;

namespace Locator.API.Storages.Interfaces;

public interface IInventoryStorage
{
    AccountInventoryItemDb[] GetInventory(string accountId, DateTime? afterDateTime = null);
    void SaveInventoryVersion(AccountInventoryItemDb inventoryItem);
    void DeleteAllInventories();
}