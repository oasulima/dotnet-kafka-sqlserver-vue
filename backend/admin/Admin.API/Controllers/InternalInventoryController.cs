using Admin.API.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shared;
using Shared.Refit;

namespace Admin.API.Controllers;

[ApiController]
public class InternalInventoryController : ControllerBase
{
    private readonly IInternalInventoryReportingService _internalInventoryReportingService;
    private readonly IProviderSettingCache _providerSettingCache;
    private readonly IInternalInventoryApi _internalInventoryApi;

    public InternalInventoryController(
        IInternalInventoryReportingService internalInventoryReportingService,
        IProviderSettingCache providerSettingCache,
        IInternalInventoryApi internalInventoryApi
    )
    {
        _internalInventoryReportingService = internalInventoryReportingService;
        _providerSettingCache = providerSettingCache;
        _internalInventoryApi = internalInventoryApi;
    }

    [HttpPost("/api/internal-inventory")]
    public async Task<
        Results<BadRequest<ModelStateDictionary>, Ok<InternalInventoryItem>>
    > AddInventoryItem([FromBody] AddInternalInventoryItemRequest request)
    {
        if (
            !(
                ValidateCurrentUserProviderId(request.Source)
                && ValidateProviderAndPrice(request.Price, request.Source)
            )
        )
        {
            return TypedResults.BadRequest(ModelState);
        }

        request.CreatingType = CreatingType.SingleEntry;
        var response = await _internalInventoryApi.Admin_AddInventory(request);

        return TypedResults.Ok(response);
    }

    [HttpGet("/api/internal-inventory/items/history")]
    public Task<IEnumerable<InternalInventoryItem>> GetHistoryInventoryItems(
        [FromQuery] string symbol
    )
    {
        return _internalInventoryReportingService.GetPreviousDaysInventoryItems(
            take: 10,
            null,
            symbol,
            DateTime.UtcNow
        );
    }

    [HttpGet("/api/internal-inventory/items")]
    public Task<IEnumerable<InternalInventoryItem>> GetInternalInventoryItems(
        [FromQuery] string? symbol = null,
        [FromQuery] CreatingType? creatingType = null,
        [FromQuery] InternalInventoryItem.State? status = null
    )
    {
        return _internalInventoryReportingService.GetInternalInventoryItems(
            symbol,
            creatingType,
            status
        );
    }

    [HttpGet("/api/internal-inventory")]
    public async Task<IList<InternalInventoryItem>> GetInfo([FromQuery] string symbol)
    {
        var response = await _internalInventoryApi.Admin_GetInventory(symbol);
        return response;
    }

    [HttpPut("/api/internal-inventory")]
    public Task UpdateInventoryItem([FromBody] InternalInventoryItem item)
    {
        return _internalInventoryApi.Admin_UpdateInventoryItem(item);
    }

    [HttpPut("/api/internal-inventory/deactivate")]
    public async Task<Results<BadRequest<ModelStateDictionary>, Ok>> MakeInactive(
        [FromBody] InternalInventoryItem item
    )
    {
        if (!ValidateCurrentUserProviderId(item.Source))
        {
            return TypedResults.BadRequest(ModelState);
        }

        await _internalInventoryApi.Admin_MakeInactive(item);

        return TypedResults.Ok();
    }

    [HttpPut("/api/internal-inventory/activate")]
    public async Task<Results<BadRequest<ModelStateDictionary>, Ok>> MakeActive(
        [FromBody] InternalInventoryItem item
    )
    {
        if (!ValidateCurrentUserProviderId(item.Source))
        {
            return TypedResults.BadRequest(ModelState);
        }

        await _internalInventoryApi.Admin_MakeActive(item);

        return TypedResults.Ok();
    }

    [HttpPut("/api/internal-inventory/delete")]
    public async Task<Results<BadRequest<ModelStateDictionary>, Ok>> DeleteInventoryItem(
        [FromBody] InternalInventoryItem item
    )
    {
        if (!ValidateCurrentUserProviderId(item.Source))
        {
            return TypedResults.BadRequest(ModelState);
        }

        await _internalInventoryApi.Admin_DeleteInventoryItem(item);

        return TypedResults.Ok();
    }

    private bool ValidateProviderAndPrice(decimal price, string source)
    {
        if (price == 0)
        {
            var provider = _providerSettingCache.GetProviderSetting(source);

            if (provider == null)
            {
                ModelState.AddModelError("Source", "Selected provider not found");
                return false;
            }
        }

        return true;
    }

    private bool ValidateCurrentUserProviderId(string providerId)
    {
        return true;
    }
}
