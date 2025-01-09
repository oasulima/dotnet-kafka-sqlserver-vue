using Admin.API.Models;
using Admin.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Admin.API.Controllers;

[ApiController]
public class OptionsController : ControllerBase
{
    private readonly IProviderSettingCache _providerSettingCache;

    public OptionsController(IProviderSettingCache providerSettingCache)
    {
        _providerSettingCache = providerSettingCache;
    }

    [HttpGet("/api/options/sources")]
    public SelectValue<string>[] GetSources()
    {
        return _providerSettingCache
            .GetActiveExternalQuery()
            .Select(x => new SelectValue<string>() { Value = x.ProviderId, Label = x.Name })
            .ToArray();
    }

    [HttpGet("/api/options/creating-types")]
    public CreatingType[] GetCreatingTypes()
    {
        return Enum.GetValues<CreatingType>().ToArray();
    }

    [HttpGet("/api/options/internal-inventory/statuses")]
    public InternalInventoryItem.State[] GetInternalInventoryStatuses()
    {
        return Enum.GetValues<InternalInventoryItem.State>().ToArray();
    }
}
