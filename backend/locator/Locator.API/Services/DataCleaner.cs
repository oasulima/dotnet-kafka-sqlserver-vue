using Locator.API.Services.Interfaces;
using Locator.API.Storages.Interfaces;

namespace Locator.API.Services;

public class DataCleaner : IDataCleaner
{
    private readonly IInventoryService _inventoryService;
    private readonly IQuoteStorage _quoteStorage;
    private readonly IInventoryStorage _inventoryStorage;
    private readonly IAutoDisableProvidersService _autoDisableProvidersService;

    public DataCleaner(IInventoryService inventoryService,
        IQuoteStorage quoteStorage,
        IInventoryStorage inventoryStorage,
        IAutoDisableProvidersService autoDisableProvidersService)
    {
        _inventoryService = inventoryService;
        _quoteStorage = quoteStorage;
        _inventoryStorage = inventoryStorage;
        _autoDisableProvidersService = autoDisableProvidersService;
    }

    public void CleanData()
    {
        using var activity = TracingConfiguration.StartActivity("DataCleaner CleanData");
        try
        {
            _inventoryService.ClearCache();
            _quoteStorage.ClearCache();
            _inventoryStorage.DeleteAllInventories();
            _autoDisableProvidersService.Clear();
        }
        catch (Exception ex)
        {
            activity.LogException(ex);
        }
    }
}