using Shared;
using Shared.Locator;

namespace Locator.API.Services.Interfaces;

public interface IInventoryService
{
    void AddLocates(string accountId, string symbol, int quantity, decimal price, string source);

    Dictionary<string, InventoryItem[]> GetInventory(string accountId);

    void ClearCache();
}