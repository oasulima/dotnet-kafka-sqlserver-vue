using Admin.API.Services.Interfaces;
using Shared.Constants;
using Shared.Refit;
using Shared.Settings;
using ProviderIdType = System.String;

namespace Admin.API.Services;

public class ProviderSettingCache : IProviderSettingCache
{
    private readonly ILocatorApi locatorApi;

    private Dictionary<ProviderIdType, ProviderSetting> _providerSettings = new();

    public ProviderSettingCache(ILocatorApi locatorApi)
    {
        this.locatorApi = locatorApi;
        RefreshSettings().GetAwaiter().GetResult();
    }

    public ProviderSetting[] GetAll()
    {
        return GetAllQuery().ToArray();
    }

    private IEnumerable<ProviderSetting> GetAllQuery()
    {
        return _providerSettings.Select(x => x.Value);
    }

    public IEnumerable<ProviderSetting> GetActiveExternalQuery()
    {
        return GetAllQuery()
            .Where(x => x.Active
                        && x.ProviderId is not Providers.SelfIds.InternalInventory);
    }

    public ProviderSetting? GetProviderSetting(string providerId)
    {
        _providerSettings.TryGetValue(providerId, out var result);
        return result;
    }

    public async Task RefreshSettings()
    {
        using var activity = TracingConfiguration.StartActivity("RefreshSettings");
        try
        {
            var freshSettings = await locatorApi.GetProviderSettings();
            var newProviderSettings = new Dictionary<ProviderIdType, ProviderSetting>();
            foreach (var settings in freshSettings)
            {
                newProviderSettings[settings.ProviderId] = settings;
            }

            _providerSettings = newProviderSettings;
        }
        catch (Exception e)
        {
            activity?.LogException(e);
            throw;
        }
    }
}