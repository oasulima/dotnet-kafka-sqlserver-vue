using Locator.API.Data.Exceptions;
using Locator.API.Data.Repositories.Interfaces;
using Locator.API.Services.Interfaces;
using Locator.API.Utility;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.API;
using Shared.Settings;

namespace Locator.API.Controllers;

[ApiController]
public class ProviderSettingController : ControllerBase
{
    private readonly IProviderSettingRepository _providerSettingRepository;
    private readonly IEventSender _eventSender;
    private readonly IAutoDisableProvidersService _autoDisableProvidersService;

    public ProviderSettingController(
        IProviderSettingRepository providerSettingRepository,
        IEventSender eventSender,
        IAutoDisableProvidersService autoDisableProvidersService
    )
    {
        _providerSettingRepository = providerSettingRepository;
        _eventSender = eventSender;
        _autoDisableProvidersService = autoDisableProvidersService;
    }

    [HttpGet("/api/settings/provider")]
    public Ok<ProviderSettingExtended[]> GetProviderSettings()
    {
        var providerSettings = _providerSettingRepository.GetAll();
        var autoDisabled = _autoDisableProvidersService.GetDisabledProviders();
        var result = providerSettings
            .Select(ps => new ProviderSettingExtended
            {
                ProviderId = ps.ProviderId,
                Name = ps.Name,
                Discount = ps.Discount,
                Active = ps.Active,
                QuoteRequestTopic = ps.QuoteRequestTopic,
                QuoteResponseTopic = ps.QuoteResponseTopic,
                BuyRequestTopic = ps.BuyRequestTopic,
                BuyResponseTopic = ps.BuyResponseTopic,
                AutoDisabled = autoDisabled.ContainsKey(ps.ProviderId)
                    ? new() { Symbols = autoDisabled[ps.ProviderId] }
                    : null,
            })
            .ToArray();
        return TypedResults.Ok(result);
    }

    [HttpPost("/api/settings/provider")]
    public Results<BadRequest<ServerErrorInfo>, Ok> AddProviderSetting(
        [FromBody] ProviderSettingRequest settingRequest
    )
    {
        try
        {
            var providerSetting = settingRequest.ToProviderSetting();
            _providerSettingRepository.Add(providerSetting);
            _eventSender.SendInvalidateCacheCommand(
                new SyncCommand.InvalidateCache(SyncCommand.CacheTypeEnum.ProviderSettings)
            );
            return TypedResults.Ok();
        }
        catch (UniqueConstraintViolationException ex)
        {
            ModelState.AddModelError(ex.ColumnName, "Should be unique");
            return TypedResults.BadRequest(
                new ServerErrorInfo { Message = ex.ColumnName, Detail = "Should be unique" }
            );
        }
    }

    [HttpPut("/api/settings/provider")]
    public Results<NotFound<string>, Ok> UpdateProviderSetting(
        [FromBody] ProviderSettingRequest settingRequest
    )
    {
        var storageProviderSetting = _providerSettingRepository.Get(settingRequest.ProviderId!);

        if (storageProviderSetting == null)
        {
            return TypedResults.NotFound("Setting not found.");
        }

        var providerSetting = settingRequest.ToProviderSetting();
        providerSetting.BuyRequestTopic = storageProviderSetting.BuyRequestTopic;
        providerSetting.BuyResponseTopic = storageProviderSetting.BuyResponseTopic;
        providerSetting.QuoteRequestTopic = storageProviderSetting.QuoteRequestTopic;
        providerSetting.QuoteResponseTopic = storageProviderSetting.QuoteResponseTopic;

        _providerSettingRepository.Update(providerSetting);

        _eventSender.SendInvalidateCacheCommand(
            new SyncCommand.InvalidateCache(SyncCommand.CacheTypeEnum.ProviderSettings)
        );

        return TypedResults.Ok();
    }

    [HttpDelete("/api/settings/provider/{providerId}")]
    public Ok DeleteProviderSetting(string providerId)
    {
        _providerSettingRepository.Delete(providerId);

        _eventSender.SendInvalidateCacheCommand(
            new SyncCommand.InvalidateCache(SyncCommand.CacheTypeEnum.ProviderSettings)
        );
        return TypedResults.Ok();
    }

    [HttpPost("/api/settings/provider/self-reg")]
    public Ok<ProviderSetting> SelfReg([FromBody] ProviderSelfRegRequest selfRegRequest)
    {
        // TODO_Review: Add validation, if provider comes in null, it excepts at the database call while it should return a 400
        var providerSetting = _providerSettingRepository.Get(selfRegRequest.Id);
        var isNew = providerSetting == null;
        providerSetting ??= new()
        {
            Active = false,
            Discount = 0,
            ProviderId = selfRegRequest.Id,
            Name = selfRegRequest.Name,
        };
        providerSetting.BuyRequestTopic = selfRegRequest.BuyRequestTopic;
        providerSetting.BuyResponseTopic = selfRegRequest.BuyResponseTopic;
        providerSetting.QuoteRequestTopic = selfRegRequest.QuoteRequestTopic;
        providerSetting.QuoteResponseTopic = selfRegRequest.QuoteResponseTopic;

        if (isNew)
        {
            _providerSettingRepository.Add(providerSetting);
        }
        else
        {
            _providerSettingRepository.Update(providerSetting);
        }

        providerSetting = _providerSettingRepository.Get(providerSetting.ProviderId);

        _eventSender.SendInvalidateCacheCommand(
            new SyncCommand.InvalidateCache(SyncCommand.CacheTypeEnum.ProviderSettings)
        );

        return TypedResults.Ok(providerSetting);
    }
}
