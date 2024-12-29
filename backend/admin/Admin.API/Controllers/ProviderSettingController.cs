using Microsoft.AspNetCore.Mvc;
using Admin.API.Services.Interfaces;
using Shared.Refit;
using Shared.Settings;

namespace Admin.API.Controllers;

[ApiController]
public class ProviderSettingController : ControllerBase
{
    private readonly IEventSender _eventSender;
    private readonly ILocatorApi _locatorApi;

    public ProviderSettingController(IEventSender eventSender, ILocatorApi locatorApi)
    {
        _eventSender = eventSender;
        _locatorApi = locatorApi;
    }

    [HttpGet("/api/settings/provider")]
    public Task<ProviderSettingExtended[]> GetProviderSettings()
    {
        return _locatorApi.GetProviderSettings();
    }

    [HttpPost("/api/settings/provider/{providerId}/activate")]
    public void ActivateProvider([FromRoute] string providerId)
    {
        _eventSender.SendEnableProviderCommand(providerId);
    }

    [HttpPost("/api/settings/provider")]
    public Task<ProviderSetting> AddProviderSetting(
        [FromBody] ProviderSettingRequest providerSettingReq)
    {
        return _locatorApi.AddProviderSetting(providerSettingReq);
    }

    [HttpPut("/api/settings/provider")]
    public Task<ProviderSetting> UpdateProviderSetting(
        [FromBody] ProviderSettingRequest providerSettingReq)
    {
        return _locatorApi.UpdateProviderSetting(providerSettingReq);
    }

    [HttpDelete("/api/settings/provider/{providerId}")]
    public Task<bool> DeleteProviderSetting([FromRoute] string providerId)
    {
        return _locatorApi.DeleteProviderSetting(providerId);
    }
}