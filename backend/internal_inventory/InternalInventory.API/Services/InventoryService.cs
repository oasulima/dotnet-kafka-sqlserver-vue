using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Shared;
using InternalInventory.API.Data.Entities;
using InternalInventory.API.Models.Options;
using InternalInventory.API.Services.Interfaces;
using InternalInventory.API.Storages.Interfaces;
using Shared;

namespace InternalInventory.API.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryStorage _inventoryStorage;
    private readonly IEventSender _eventSender;

    public InventoryService(IInventoryStorage inventoryStorage, IEventSender eventSender,
        IOptions<AppOptions> appOptions)
    {
        _inventoryStorage = inventoryStorage;
        _eventSender = eventSender;
        RestoreData(appOptions.Value.DayDataCleanupTimeUtc);
    }

    private void RestoreData(TimeOnly dayDataCleanupTimeUtc)
    {
        var now = DateTime.UtcNow;
        var from = now.Date.Add(dayDataCleanupTimeUtc.ToTimeSpan());
        if (from > now)
        {
            from = from.AddDays(-1);
        }

        var inventories = _inventoryStorage.GetInventoryFromDb(from);
        foreach (var (_, dbItem) in inventories)
        {
            var item = Map(dbItem);

            _inventoryCache[item.Id] = item;
            _inventoryBySymbol.GetOrAdd(item.Symbol, _ => new())
                .Add(item);
            _inventoryBySource.GetOrAdd(new SymbolSourceKey(item.Symbol, item.Source), _ => new())
                .Add(item);
        }
    }

    private readonly ConcurrentDictionary<string, InternalInventoryItem> _inventoryCache = new();
    private readonly ConcurrentDictionary<string, ConcurrentBag<InternalInventoryItem>> _inventoryBySymbol = new();

    private record SymbolSourceKey(string Symbol, string Source);

    private readonly ConcurrentDictionary<SymbolSourceKey, ConcurrentBag<InternalInventoryItem>> _inventoryBySource =
        new();

    public IList<InternalInventoryItem> GetInventory(string symbol, string? providerId)
    {
        _inventoryBySymbol.TryGetValue(symbol, out var result);

        return result?.Where(x => providerId == null || x.Source == providerId).ToArray() ?? Array.Empty<InternalInventoryItem>();
    }

    public InternalInventoryItem AddInventory(InternalInventoryItem item)
    {
        InternalInventoryItemDb dbItem;
        lock (item)
        {
            item.Id = GetNewId();
            item.Version = 1;
            item.CreatedAt = DateTime.UtcNow;
            _inventoryCache[item.Id] = item;
            var bag = _inventoryBySymbol.GetOrAdd(item.Symbol, _ => new());
            bag.Add(item);
            bag = _inventoryBySource.GetOrAdd(new SymbolSourceKey(item.Symbol, item.Source), _ => new());
            bag.Add(item);

            dbItem = Map(item);
        }

        SaveInventoryItem(dbItem);
        return item;
    }

    public InternalInventoryItem AddInventory(AddInternalInventoryItemRequest request)
    {
        return AddInventory(new InternalInventoryItem
        {
            Quantity = request.Quantity,
            Price = request.Price,
            Source = request.Source,
            Symbol = request.Symbol,
            CreatingType = request.CreatingType,
        });
    }

    public InternalInventoryItem? UpdateInventory(InternalInventoryItem newItem)
    {
        if (_inventoryCache.TryGetValue(newItem.Id, out var item))
        {
            InternalInventoryItemDb dbItem;
            lock (item)
            {
                if (item.Version == newItem.Version &&
                    item.Symbol == newItem.Symbol &&
                    item.Source == newItem.Source &&
                    item.Status == InternalInventoryItem.State.Inactive)
                {
                    item.Price = newItem.Price;
                    item.Quantity = newItem.Quantity;

                    item.Version++;
                    dbItem = Map(item);
                }
                else
                {
                    throw new Exception("item cannot be changed"); //TODO: correct message
                }
            }

            SaveInventoryItem(dbItem);
        }

        return item;
    }

    public InternalInventoryItem? MakeInactive(string id)
    {
        if (_inventoryCache.TryGetValue(id, out var item))
        {
            InternalInventoryItemDb dbItem;
            lock (item)
            {
                if (item.Status == InternalInventoryItem.State.Active)
                {
                    item.Status = InternalInventoryItem.State.Inactive;
                    item.Version++;
                }
                else
                {
                    throw new Exception(
                        $"Item cannot become inactive. Id: {item.Id}, Status: {item.Status}, Version: {item.Version}"); //TODO: correct message
                }

                dbItem = Map(item);
            }

            SaveInventoryItem(dbItem);
        }

        return item;
    }

    public InternalInventoryItem? MakeActive(string id, int version)
    {
        if (_inventoryCache.TryGetValue(id, out var item))
        {
            InternalInventoryItemDb dbItem;
            lock (item)
            {
                if (item.Version == version && item.Status == InternalInventoryItem.State.Inactive)
                {
                    item.Status = InternalInventoryItem.State.Active;

                    item.Version++;
                }
                else
                {
                    throw new Exception("Item cannot be marked as active"); //TODO: correct message
                }

                dbItem = Map(item);
            }

            SaveInventoryItem(dbItem);
        }

        return item;
    }

    public InternalInventoryItem? DeleteInventoryItem(string id, int version)
    {
        if (_inventoryCache.TryGetValue(id, out var item))
        {
            InternalInventoryItemDb dbItem;
            lock (item)
            {
                if (item.Version == version && item.Status == InternalInventoryItem.State.Inactive)
                {
                    item.Status = InternalInventoryItem.State.Deleted;

                    item.Version++;
                }
                else
                {
                    throw new Exception(
                        $"Item cannot be deleted. Id: {item.Id}, Status: {item.Status}, Version: {item.Version}"); //TODO: correct message
                }

                dbItem = Map(item);
            }

            SaveInventoryItem(dbItem);
        }

        return item;
    }

    public void ProcessQuoteRequest(ProviderQuoteRequest quoteRequest)
    {
        if (DateTime.UtcNow > quoteRequest.ValidTill)
        {
            return;
        }

        var response = new ProviderQuoteResponse()
        {
            Id = quoteRequest.Id,
            Status = ProviderQuoteResponse.StatusEnum.Ready,
            Symbol = quoteRequest.Symbol,
            AccountId = quoteRequest.AccountId,
            QuoteId = quoteRequest.QuoteId
        };
        if (_inventoryBySymbol.TryGetValue(quoteRequest.Symbol, out var inventory))
        {
            response.Prices = inventory
                .Where(x => x.Status == InternalInventoryItem.State.Active &&
                            x.Quantity > 0)
                .Select(x => new PriceInfo()
                {
                    Price = x.Price,
                    Quantity = x.Quantity,
                    Source = x.Source
                }).ToList();

            if (!response.Prices.Any(x => x.Price > 0))
            {
                var priceInfo = new PriceInfo()
                {
                    Quantity = 0,
                };
                foreach (var inv in inventory.Where(x => x.Status == InternalInventoryItem.State.Active))
                {
                    if (inv.Price > priceInfo.Price)
                    {
                        priceInfo.Price = inv.Price;
                        priceInfo.Source = inv.Source;
                    }
                }

                if (priceInfo.Price > 0)
                {
                    response.Prices.Add(priceInfo);
                }
            }
        }


        _eventSender.SendQuoteResponseReadyEvent(response);
    }

    public void ProcessBuyRequest(ProviderBuyRequest buyRequest)
    {
        //TODO: try/catch?? transactional?
        if (DateTime.UtcNow > buyRequest.ValidTill)
        {
            return;
        }

        var response = new ProviderBuyResponse()
        {
            Id = buyRequest.Id,
            Symbol = buyRequest.Symbol,
            AccountId = buyRequest.AccountId,
            QuoteId = buyRequest.QuoteId
        };
        var boughtItems = new List<PriceInfo>(buyRequest.RequestedAssets.Count);
        response.BoughtAssets = boughtItems;
        var affectedInventoryItems = new List<InternalInventoryItemDb>();

        var symbol = buyRequest.Symbol;
        foreach (var requestedAsset in buyRequest.RequestedAssets)
        {
            ConcurrentBag<InternalInventoryItem>? bag;
            if (requestedAsset.Source == null)
            {
                _inventoryBySymbol.TryGetValue(symbol, out bag);
            }
            else
            {
                var key = new SymbolSourceKey(symbol, requestedAsset.Source);
                _inventoryBySource.TryGetValue(key, out bag);
            }

            if (bag == null)
            {
                continue;
            }

            //We can store sorted list if any perf issues.
            var sortedInventoryItems = bag.OrderBy(x => x.Price);

            var quantityToBuy = requestedAsset.Quantity;
            foreach (var inventoryItem in sortedInventoryItems)
            {
                if (quantityToBuy <= 0)
                {
                    break;
                }

                if (inventoryItem.Status != InternalInventoryItem.State.Active ||
                    inventoryItem.Price > requestedAsset.Price ||
                    inventoryItem.Quantity <= 0)
                {
                    continue;
                }

                lock (inventoryItem) //TryEnter with a few milliseconds if any perf issues
                {
                    if (inventoryItem.Status == InternalInventoryItem.State.Active &&
                        inventoryItem.Price <= requestedAsset.Price)
                    {
                        var q = Math.Min(inventoryItem.Quantity, quantityToBuy);
                        if (q <= 0)
                        {
                            continue;
                        }

                        boughtItems.Add(new PriceInfo()
                        {
                            Price = inventoryItem.Price == 0
                                ? requestedAsset.Price
                                : inventoryItem.Price,
                            Quantity = q,
                            Source = inventoryItem.Source
                        });
                        inventoryItem.Quantity -= q;
                        inventoryItem.SoldQuantity += q;
                        inventoryItem.Version++;

                        affectedInventoryItems.Add(Map(inventoryItem));
                        quantityToBuy -= q;
                    }
                }
            }
        }

        var requestedAmount = buyRequest.RequestedAssets.Sum(x => x.Quantity);
        var fulfilledAmount = response.BoughtAssets.Sum(x => x.Quantity);
        if (requestedAmount == fulfilledAmount)
        {
            response.Status = ProviderBuyResponse.StatusEnum.Fulfilled;
        }
        else if (fulfilledAmount > 0)
        {
            response.Status = ProviderBuyResponse.StatusEnum.Partial;
        }
        else
        {
            response.Status = ProviderBuyResponse.StatusEnum.Failed;
            response.RejectCode = "NoInventory";
        }

        //Send buy response first to unblock end user, 
        _eventSender.SendBuyResponseEvent(response);

        SaveInventoryItems(affectedInventoryItems);
    }

    private void SaveInventoryItem(InternalInventoryItemDb dbItem)
    {
        _eventSender.SendInternalInventoryItemChangeEvent(Map(dbItem));
        _inventoryStorage.SaveCurrentVersion(dbItem);
    }

    private void SaveInventoryItems(IList<InternalInventoryItemDb> dbItems)
    {
        foreach (var dbItem in dbItems)
        {
            _eventSender.SendInternalInventoryItemChangeEvent(Map(dbItem));
        }

        _inventoryStorage.SaveInventoryItems(dbItems);
    }

    public void ClearCache()
    {
        _inventoryCache.Clear();
        _inventoryBySymbol.Clear();
        _inventoryBySource.Clear();
    }

    private InternalInventoryItem Map(InternalInventoryItemDb dbItem)
    {
        return new InternalInventoryItem()
        {
            Id = dbItem.Id,
            Price = dbItem.Price,
            Quantity = dbItem.Quantity,
            Source = dbItem.Source,
            Status = Enum.Parse<InternalInventoryItem.State>(dbItem.Status),
            Symbol = dbItem.Symbol,
            Tag = dbItem.Tag,
            CoveredInvItemId = dbItem.CoveredInvItemId,
            Version = dbItem.Version,
            CreatingType = dbItem.CreatingType,
            CreatedAt = dbItem.CreatedAt,
            SoldQuantity = dbItem.SoldQuantity
        };
    }

    private InternalInventoryItemDb Map(InternalInventoryItem item)
    {
        return new InternalInventoryItemDb()
        {
            Id = item.Id,
            Price = item.Price,
            Quantity = item.Quantity,
            Source = item.Source,
            Status = item.Status.ToString(),
            Symbol = item.Symbol,
            Tag = item.Tag,
            CoveredInvItemId = item.CoveredInvItemId,
            Version = item.Version,
            CreatingType = item.CreatingType,
            CreatedAt = item.CreatedAt,
            SoldQuantity = item.SoldQuantity
        };
    }

    string GetNewId()
    {
        return Guid.NewGuid().ToString();
    }
}