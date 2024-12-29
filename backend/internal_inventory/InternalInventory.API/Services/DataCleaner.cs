using InternalInventory.API.Services.Interfaces;
using InternalInventory.API.Storages.Interfaces;

namespace InternalInventory.API.Services;

public class DataCleaner(
    IInventoryService inventoryService,
    IInventoryStorage inventoryStorage
) : IDataCleaner
{
    public void CleanData()
    {
        inventoryService.ClearCache();
        inventoryStorage.DeleteAllInventoryItems();
    }
}