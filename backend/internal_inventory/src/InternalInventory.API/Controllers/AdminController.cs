using InternalInventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace InternalInventory.API.Controllers;

[ApiController]
public class AdminController : ControllerBase
{
    private readonly IInventoryService _service;

    public AdminController(IInventoryService service)
    {
        _service = service;
    }

    [HttpGet("/api/Admin/GetInventory")]
    public Ok<IList<InternalInventoryItem>> GetInventory(
        [FromQuery] string symbol,
        [FromQuery] string? providerId = null
    )
    {
        var result = _service.GetInventory(symbol, providerId);
        return TypedResults.Ok(result);
    }

    [HttpPost("/api/Admin/AddInventory")]
    public Ok<InternalInventoryItem> AddInventory([FromBody] AddInternalInventoryItemRequest item)
    {
        return TypedResults.Ok(_service.AddInventory(item));
    }

    [HttpPost("/api/Admin/UpdateInventoryItem")]
    public Ok UpdateInventoryItem([FromBody] InternalInventoryItem item)
    {
        _service.UpdateInventory(item);
        return TypedResults.Ok();
    }

    [HttpPost("/api/Admin/MakeInactive")]
    public Ok MakeInactive([FromBody] InternalInventoryItem item)
    {
        _service.MakeInactive(item.Id);
        return TypedResults.Ok();
    }

    [HttpPost("/api/Admin/MakeActive")]
    public Ok MakeActive([FromBody] InternalInventoryItem item)
    {
        _service.MakeActive(item.Id, item.Version);
        return TypedResults.Ok();
    }

    [HttpPost("/api/Admin/DeleteInventoryItem")]
    public Ok DeleteInventoryItem([FromBody] InternalInventoryItem item)
    {
        _service.DeleteInventoryItem(item.Id, item.Version);
        return TypedResults.Ok();
    }
}
