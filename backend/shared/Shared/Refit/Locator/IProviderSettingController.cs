using Shared.Settings;
using Refit;

namespace Shared.Refit.Locator;

public interface IProviderSettingController
{
    [Get("/api/settings/provider")]
    Task<ProviderSettingExtended[]> GetProviderSettings();

    [Post("/api/settings/provider")]
    Task<ProviderSetting> AddProviderSetting([Body] ProviderSettingRequest settingRequest);

    [Put("/api/settings/provider")]
    Task<ProviderSetting> UpdateProviderSetting([Body] ProviderSettingRequest settingRequest);

    [Delete("/api/settings/provider/{providerId}")]
    Task<bool> DeleteProviderSetting(string providerId);

    [Post("/api/settings/provider/self-reg")]
    Task<ProviderSetting> SelfReg([Body] ProviderSelfRegRequest selfRegRequest);
}