using TradeZero.Locator.Emulator.Options;
using Shared.Settings;
using Refit;
using Shared.Refit;
using Shared.Constants;

namespace TradeZero.Locator.Emulator.Services;

public class ProviderSettingsApi
{
    private readonly RandomHelper randomHelper;
    private readonly ILocatorApi locatorApi;

    public ProviderSettingsApi(ApiOptions apiOptions, RandomHelper randomHelper)
    {
        this.randomHelper = randomHelper;
        this.locatorApi = RestService.For<ILocatorApi>(apiOptions.LocatorUrl);
    }

    public async Task InitProviders()
    {
        var providers = await locatorApi.GetProviderSettings();
        var providerIds = providers.Select(x => x.ProviderId).ToArray();
        var tasks = randomHelper.GetSources()
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
        var ii = providers.Where(x => x.ProviderId == Providers.SelfIds.InternalInventory).FirstOrDefault();
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
            $"provider setting: Name: {providerSettingReq.Name}; Active: {providerSettingReq.Active}; Discount: {providerSettingReq.Discount}");
        return locatorApi.AddProviderSetting(providerSettingReq);
    }
}