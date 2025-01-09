using System.Collections.Concurrent;
using Locator.API.Data.Entities;
using Locator.API.Services.Interfaces;
using Locator.API.Storages.Interfaces;
using Shared.Locator;

namespace Locator.API.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryStorage _inventoryStorage;
    private readonly ITimeService _timeService;

    private record AccountIdentifier(string AccountId);

    private class AccountInventoryInternal
    {
        public ConcurrentDictionary<string, ConcurrentBag<InventoryItem>> Assets { get; } = new();
    }

    private readonly ConcurrentDictionary<AccountIdentifier, AccountInventoryInternal> _cache =
        new();

    public InventoryService(IInventoryStorage inventoryStorage, ITimeService timeService)
    {
        _inventoryStorage = inventoryStorage;
        _timeService = timeService;
    }

    private AccountInventoryInternal GetAccountInventory(string accountId)
    {
        var key = new AccountIdentifier(accountId);
        var inventory = _cache.GetOrAdd(key, InitializeAccountInventory);
        return inventory;
    }

    private AccountInventoryInternal InitializeAccountInventory(AccountIdentifier accountIdentifier)
    {
        var result = new AccountInventoryInternal();
        var from = _timeService.GetPreviousCleanupTimeInUtc(DateTime.UtcNow);
        var dbInventory = _inventoryStorage.GetInventory(accountIdentifier.AccountId, from);
        foreach (var value in dbInventory)
        {
            var inventoryItem = Map(value);
            result.Assets.GetOrAdd(value.Symbol, _ => new()).Add(inventoryItem);
        }

        return result;
    }

    private ConcurrentBag<InventoryItem> GetSymbolInventory(string accountId, string symbol)
    {
        var inventory = GetAccountInventory(accountId);

        return inventory.Assets.GetOrAdd(symbol, _ => new());
    }

    public void AddLocates(
        string accountId,
        string symbol,
        int quantity,
        decimal price,
        string source
    )
    {
        var inventory = GetSymbolInventory(accountId, symbol);
        var inventoryItem = new InventoryItem
        {
            Id = Guid.NewGuid().ToString(),
            Symbol = symbol,
            Version = 1,
            AccountId = accountId,
            AvailableQuantity = quantity,
            LocatedQuantity = quantity,
        };
        AccountInventoryItemDb inventoryItemDb;
        lock (inventory)
        {
            inventory.Add(inventoryItem);

            inventoryItemDb = Map(inventoryItem);
        }

        _inventoryStorage.SaveInventoryVersion(inventoryItemDb);
    }

    public Dictionary<string, InventoryItem[]> GetInventory(string accountId)
    {
        var accountInventory = GetAccountInventory(accountId);
        return accountInventory.Assets.ToDictionary(x => x.Key, x => x.Value.ToArray());
    }

    public void ClearCache()
    {
        _cache.Clear();
    }

    private AccountInventoryItemDb Map(InventoryItem inventoryItem)
    {
        return new AccountInventoryItemDb
        {
            Id = inventoryItem.Id,
            Symbol = inventoryItem.Symbol,
            Version = inventoryItem.Version,
            AccountId = inventoryItem.AccountId,
            AvailableQuantity = inventoryItem.AvailableQuantity,
            LocatedQuantity = inventoryItem.LocatedQuantity,
        };
    }

    private InventoryItem Map(AccountInventoryItemDb inventoryItem)
    {
        return new InventoryItem
        {
            Id = inventoryItem.Id,
            Symbol = inventoryItem.Symbol,
            Version = inventoryItem.Version,
            AccountId = inventoryItem.AccountId,
            AvailableQuantity = inventoryItem.AvailableQuantity,
            LocatedQuantity = inventoryItem.LocatedQuantity,
        };
    }
}
