using System.Collections.Concurrent;
using Locator.API.Data.Repositories.Interfaces;
using Locator.API.Storages.Interfaces;
using Shared.Settings;

namespace Locator.API.Storages;

public class ProviderSettingStorage : IProviderSettingStorage
{
    protected Func<ProviderSetting, string> GetKey => x => x.ProviderId;
    protected readonly IProviderSettingRepository Repository;
    protected ConcurrentDictionary<string, ProviderSetting> Storage;

    public ProviderSettingStorage(IProviderSettingRepository repository)
    {
        Storage = new ConcurrentDictionary<string, ProviderSetting>(StringComparer.OrdinalIgnoreCase);
        Repository = repository;
        RefreshStorage();
    }

    public ProviderSetting[] GetAll()
    {
        return Storage.Select(x => x.Value).ToArray();
    }

    public ProviderSetting? Get(string settingKey)
    {
        Storage.TryGetValue(settingKey, out var value);
        return value;
    }

    public void RefreshStorage()
    {
        var freshSettings = Repository.GetAll();
        var freshKeys = freshSettings.Select(x => GetKey(x));
        var keysToRemove = Storage.Select(x => x.Key).Except(freshKeys);

        foreach (var key in keysToRemove)
        {
            Storage.Remove(key, out _);
        }

        foreach (var setting in freshSettings)
        {
            if (Storage.TryGetValue(GetKey(setting), out var oldSetting))
            {
                if (IsChanged(setting, oldSetting))
                {
                    Storage[GetKey(setting)] = setting;
                }
            }
            else
            {
                Storage[GetKey(setting)] = setting;
            }
        }
    }

    private bool IsChanged(ProviderSetting setting, ProviderSetting oldSetting)
    {
        //TODO: check if nowadays caching of Properties metadata bust performance
        foreach (var property in typeof(ProviderSetting).GetProperties())
        {
            var newValue = property.GetValue(setting);
            var oldValue = property.GetValue(oldSetting);

            if (newValue == null)
            {
                if (oldValue == null)
                {
                    continue;
                }

                return true;
            }

            if (!newValue.Equals(oldValue))
            {
                return true;
            }
        }

        return false;
    }
}