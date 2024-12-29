using Microsoft.AspNetCore.Mvc;
using Shared.Locator;
using Shared.Refit;

namespace Admin.API.Controllers;

[ApiController]
public class InventoryController : ControllerBase
{
    private readonly ILocatorApi _locatorApi;

    public InventoryController(ILocatorApi locatorApi)
    {
        _locatorApi = locatorApi;
    }

    [HttpPost("/api/Inventory/inventory")]
    public Task<Dictionary<string, InventoryItem[]>> Get([FromBody] InventoryRequest request)
    {
        return _locatorApi.GetInventory(request);
    }
}