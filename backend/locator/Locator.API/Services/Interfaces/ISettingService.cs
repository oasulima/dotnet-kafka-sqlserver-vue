using Shared.Settings;

namespace Locator.API.Services.Interfaces;

public interface ISettingService
{
    ProviderSetting[] GetActiveExternalProviderSettings();
}