using Microsoft.AspNetCore.Mvc;
using Reporting.API.Data.Repositories.Interfaces;
using Reporting.API.Utility;
using Shared;

namespace Reporting.API.Controllers;

[ApiController]
public class InternalInventoryItemController : ControllerBase
{
    private readonly IInternalInventoryItemRepository _internalInventoryItemRepository;

    public InternalInventoryItemController(IInternalInventoryItemRepository internalInventoryItemRepository)
    {
        _internalInventoryItemRepository = internalInventoryItemRepository;
    }

    [HttpGet("/api/internal-inventory/items/history")]
    public InternalInventoryItem[] GetInternalInventoryItemsHistory([FromQuery] int take,
        [FromQuery] string? symbol = null, [FromQuery] string? providerId = null,
        [FromQuery] DateTime? beforeCreatedAt = null)
    {
        return _internalInventoryItemRepository.GetInternalInventoryItemsHistory(take, providerId, symbol, beforeCreatedAt)
            .Select(Mappers.ToInternalInventoryItem).ToArray();
    }

    [HttpGet("/api/internal-inventory/items")]
    public InternalInventoryItem[] GetInternalInventoryItems([FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null, [FromQuery] string? symbol = null, [FromQuery] CreatingType? creatingType = null,
        [FromQuery] InternalInventoryItem.State? status = null)
    {
        return _internalInventoryItemRepository.GetInternalInventoryItems(from, to, symbol, creatingType,
                status)
            .Select(Mappers.ToInternalInventoryItem).ToArray();
    }
}