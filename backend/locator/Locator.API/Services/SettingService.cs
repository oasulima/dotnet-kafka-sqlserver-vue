using Locator.API.Services.Interfaces;
using Locator.API.Storages.Interfaces;
using Shared.Settings;

namespace Locator.API.Services;

public class SettingService(
        IProviderSettingStorage providerSettingStorage
    ) : ISettingService
{
    public ProviderSetting[] GetActiveExternalProviderSettings()
    {
        return providerSettingStorage.GetAll()
            .Where(provider => provider.Active && provider.BuyRequestTopic != null).ToArray();
    }
}