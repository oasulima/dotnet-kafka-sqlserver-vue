using Shared.Settings;

namespace Locator.API.Data.Repositories.Interfaces;

public interface IProviderSettingRepository
{
    ProviderSetting? Get(string providerId);
    List<ProviderSetting> GetAll();
    void Add(ProviderSetting providerSetting);
    void Update(ProviderSetting providerSetting);
    void Delete(string providerId);
}
