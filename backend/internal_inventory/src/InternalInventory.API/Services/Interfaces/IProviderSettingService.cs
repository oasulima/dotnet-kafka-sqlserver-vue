using Shared.Settings;

namespace InternalInventory.API.Services.Interfaces;

public interface IProviderSettingService
{
    Task<ProviderSettingExtended[]> GetProviderSettingsAsync();
}
