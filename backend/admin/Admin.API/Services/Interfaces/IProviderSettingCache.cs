using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Settings;

namespace Admin.API.Services.Interfaces;

public interface IProviderSettingCache
{
    public IEnumerable<ProviderSetting> GetActiveExternalQuery();

    public ProviderSetting? GetProviderSetting(string providerId);

    public Task RefreshSettings();
}