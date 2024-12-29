using Refit;
using Shared.Locator;

namespace Shared.Refit.Locator;

public interface IInventoryController
{
    [Post("/api/Inventory")]
    Task<Dictionary<string, InventoryItem[]>> GetInventory([Body] InventoryRequest request);
}