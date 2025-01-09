using InternalInventory.API.Services.Interfaces;
using Shared.Refit;
using Shared.Settings;

namespace InternalInventory.API.Services;

public class ProviderSettingService : IProviderSettingService
{
    private readonly ILocatorApi _locatorApi;

    public ProviderSettingService(ILocatorApi locatorApi)
    {
        _locatorApi = locatorApi;
    }

    public Task<ProviderSettingExtended[]> GetProviderSettingsAsync()
    {
        return _locatorApi.GetProviderSettings();
    }
}
