using Shared.Settings;

namespace Locator.API.Storages.Interfaces;

public interface IProviderSettingStorage
{
    public ProviderSetting[] GetAll();
    public ProviderSetting? Get(string key);

    public void RefreshStorage();
}