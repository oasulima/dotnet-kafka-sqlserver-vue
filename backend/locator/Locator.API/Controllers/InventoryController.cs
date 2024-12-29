using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Locator.API.Services.Interfaces;
using Shared.Locator;

namespace Locator.API.Controllers;

[ApiController]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;

    public InventoryController(IInventoryService service)
    {
        _service = service;
    }

    [HttpPost("/api/Inventory")]
    public Ok<Dictionary<string, InventoryItem[]>> Get([FromBody] InventoryRequest request)
    {
        var inventory = _service.GetInventory(request.AccountId);

        return TypedResults.Ok(inventory);
    }
}