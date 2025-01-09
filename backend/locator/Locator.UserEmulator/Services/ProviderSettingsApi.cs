using Locator.UserEmulator;
using Locator.UserEmulator.Options;
using Refit;
using Shared.Constants;
using Shared.Refit;
using Shared.Settings;

namespace Locator.UserEmulator.Services;

public class ProviderSettingsApi
{
    private readonly RandomHelper randomHelper;
    private readonly ILocatorApi locatorApi;

    public ProviderSettingsApi(ApiOptions apiOptions, RandomHelper randomHelper)
    {
        this.randomHelper = randomHelper;
        locatorApi = RestService.For<ILocatorApi>(apiOptions.LocatorUrl);
    }

    public async Task InitProviders()
    {
        var providers = await locatorApi.GetProviderSettings();
        var providerIds = providers.Select(x => x.ProviderId).ToArray();
        var tasks = randomHelper
            .GetSources()
            .Except(providerIds)
            .Select(source => new ProviderSettingRequest
            {
                Name = source,
                Active = true,
                Discount = randomHelper.GetRandomProviderDiscount(),
                ProviderId = source,
            })
            .Select(AddProviderSetting);

        Task.WaitAll(tasks);
        var ii = providers
            .Where(x => x.ProviderId == Providers.SelfIds.InternalInventory)
            .FirstOrDefault();
        if (ii != null)
        {
            var request = new ProviderSettingRequest
            {
                Name = ii.Name,
                Active = true,
                Discount = ii.Discount,
                ProviderId = ii.ProviderId,
            };
            await locatorApi.UpdateProviderSetting(request);
        }
    }

    public Task AddProviderSetting(ProviderSettingRequest providerSettingReq)
    {
        SharedData.Log(
            $"provider setting: Name: {providerSettingReq.Name}; Active: {providerSettingReq.Active}; Discount: {providerSettingReq.Discount}"
        );
        return locatorApi.AddProviderSetting(providerSettingReq);
    }
}
