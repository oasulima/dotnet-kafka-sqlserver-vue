using System.Collections.Concurrent;
using InternalInventory.API.Data.Entities;
using InternalInventory.API.Data.Repositories;
using InternalInventory.API.Models.Options;
using InternalInventory.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using Shared;

namespace InternalInventory.API.Services;

public class InventoryService : IInventoryService
{
    private record SymbolSourceKey(string Symbol, string Source);

    private readonly IInternalInventoryItemRepository internalInventoryItemRepository;
    private readonly IEventSender eventSender;
    private readonly ConcurrentDictionary<string, InternalInventoryItem> itemsByID = new();
    private readonly ConcurrentDictionary<
        string,
        ConcurrentBag<InternalInventoryItem>
    > itemsBySymbol = new();

    private readonly ConcurrentDictionary<
        SymbolSourceKey,
        ConcurrentBag<InternalInventoryItem>
    > itemsBySource = new();

    public InventoryService(
        IEventSender eventSender,
        IOptions<AppOptions> appOptions,
        IInternalInventoryItemRepository internalInventoryItemRepository
    )
    {
        this.internalInventoryItemRepository = internalInventoryItemRepository;
        this.eventSender = eventSender;
        RestoreData(appOptions.Value.DayDataCleanupTimeUtc, internalInventoryItemRepository);
    }

    private void RestoreData(
        TimeOnly dayDataCleanupTimeUtc,
        IInternalInventoryItemRepository internalInventoryItemRepository
    )
    {
        var now = DateTime.UtcNow;
        var from = now.Date.Add(dayDataCleanupTimeUtc.ToTimeSpan());
        if (from > now)
        {
            from = from.AddDays(-1);
        }

        var inventories = GetItemsWithLatestVersions(from, internalInventoryItemRepository);
        foreach (var dbItem in inventories)
        {
            var item = Map(dbItem);

            itemsByID[item.Id] = item;
            itemsBySymbol.GetOrAdd(item.Symbol, _ => new()).Add(item);
            itemsBySource
                .GetOrAdd(new SymbolSourceKey(item.Symbol, item.Source), _ => new())
                .Add(item);
        }
    }

    private IReadOnlyCollection<InternalInventoryItemDb> GetItemsWithLatestVersions(
        DateTime from,
        IInternalInventoryItemRepository internalInventoryItemRepository
    )
    {
        var inventoryItems = internalInventoryItemRepository.Get(from);
        var result = new Dictionary<string, InternalInventoryItemDb>();

        foreach (var dbItem in inventoryItems)
        {
            if (!result.TryGetValue(dbItem.Id, out var item) || item.Version < dbItem.Version)
            {
                result[dbItem.Id] = dbItem;
            }
        }

        return result.Values;
    }

    public IList<InternalInventoryItem> GetInventory(string symbol, string? providerId)
    {
        itemsBySymbol.TryGetValue(symbol, out var result);

        return result?.Where(x => providerId == null || x.Source == providerId).ToArray()
            ?? Array.Empty<InternalInventoryItem>();
    }

    private InternalInventoryItem AddInventory(InternalInventoryItem item)
    {
        InternalInventoryItemDb dbItem;
        lock (item)
        {
            itemsByID[item.Id] = item;
            var bag = itemsBySymbol.GetOrAdd(item.Symbol, _ => new());
            bag.Add(item);
            bag = itemsBySource.GetOrAdd(new SymbolSourceKey(item.Symbol, item.Source), _ => new());
            bag.Add(item);

            dbItem = Map(item);
        }

        SaveInventoryItem(dbItem);
        return item;
    }

    public InternalInventoryItem AddInventory(AddInternalInventoryItemRequest request)
    {
        return AddInventory(
            new InternalInventoryItem
            {
                Id = Guid.NewGuid().ToString(),
                Quantity = request.Quantity,
                Price = request.Price,
                Source = request.Source,
                Symbol = request.Symbol,
                CreatingType = request.CreatingType,
                Version = 1,
                CreatedAt = DateTime.UtcNow,
                SoldQuantity = 0,
                Status = InternalInventoryItem.State.Active,
            }
        );
    }

    public InternalInventoryItem? UpdateInventory(InternalInventoryItem newItem)
    {
        if (itemsByID.TryGetValue(newItem.Id, out var item))
        {
            InternalInventoryItemDb dbItem;
            lock (item)
            {
                if (
                    item.Version == newItem.Version
                    && item.Symbol == newItem.Symbol
                    && item.Source == newItem.Source
                    && item.Status == InternalInventoryItem.State.Inactive
                )
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
        if (itemsByID.TryGetValue(id, out var item))
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
                        $"Item cannot become inactive. Id: {item.Id}, Status: {item.Status}, Version: {item.Version}"
                    ); //TODO: correct message
                }

                dbItem = Map(item);
            }

            SaveInventoryItem(dbItem);
        }

        return item;
    }

    public InternalInventoryItem? MakeActive(string id, int version)
    {
        if (itemsByID.TryGetValue(id, out var item))
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
        if (itemsByID.TryGetValue(id, out var item))
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
                        $"Item cannot be deleted. Id: {item.Id}, Status: {item.Status}, Version: {item.Version}"
                    ); //TODO: correct message
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

        var prices = Array.Empty<PriceInfo>();

        if (itemsBySymbol.TryGetValue(quoteRequest.Symbol, out var inventory))
        {
            prices = inventory
                .Where(x => x.Status == InternalInventoryItem.State.Active && x.Quantity > 0)
                .Select(x => new PriceInfo()
                {
                    Price = x.Price,
                    Quantity = x.Quantity,
                    Source = x.Source,
                })
                .ToArray();
        }

        var response = new ProviderQuoteResponse()
        {
            Id = quoteRequest.Id,
            Status = ProviderQuoteResponse.StatusEnum.Ready,
            Symbol = quoteRequest.Symbol,
            AccountId = quoteRequest.AccountId,
            QuoteId = quoteRequest.QuoteId,
            Prices = prices,
        };

        eventSender.SendQuoteResponseReadyEvent(response);
    }

    public void ProcessBuyRequest(ProviderBuyRequest buyRequest)
    {
        //TODO: try/catch?? transactional?
        if (DateTime.UtcNow > buyRequest.ValidTill)
        {
            return;
        }

        var boughtItems = new List<PriceInfo>(buyRequest.RequestedAssets.Count);
        var affectedInventoryItems = new List<InternalInventoryItemDb>();

        var symbol = buyRequest.Symbol;
        foreach (var requestedAsset in buyRequest.RequestedAssets)
        {
            ConcurrentBag<InternalInventoryItem>? bag;
            if (requestedAsset.Source == null)
            {
                itemsBySymbol.TryGetValue(symbol, out bag);
            }
            else
            {
                var key = new SymbolSourceKey(symbol, requestedAsset.Source);
                itemsBySource.TryGetValue(key, out bag);
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

                if (
                    inventoryItem.Status != InternalInventoryItem.State.Active
                    || inventoryItem.Price > requestedAsset.Price
                    || inventoryItem.Quantity <= 0
                )
                {
                    continue;
                }

                lock (inventoryItem) //TryEnter with a few milliseconds if any perf issues
                {
                    if (
                        inventoryItem.Status == InternalInventoryItem.State.Active
                        && inventoryItem.Price <= requestedAsset.Price
                    )
                    {
                        var q = Math.Min(inventoryItem.Quantity, quantityToBuy);
                        if (q <= 0)
                        {
                            continue;
                        }

                        boughtItems.Add(
                            new PriceInfo()
                            {
                                Price =
                                    inventoryItem.Price == 0
                                        ? requestedAsset.Price
                                        : inventoryItem.Price,
                                Quantity = q,
                                Source = inventoryItem.Source,
                            }
                        );
                        inventoryItem.Quantity -= q;
                        inventoryItem.SoldQuantity += q;
                        inventoryItem.Version++;

                        affectedInventoryItems.Add(Map(inventoryItem));
                        quantityToBuy -= q;
                    }
                }
            }
        }

        var response = new ProviderBuyResponse()
        {
            Id = buyRequest.Id,
            Symbol = buyRequest.Symbol,
            AccountId = buyRequest.AccountId,
            QuoteId = buyRequest.QuoteId,
            BoughtAssets = boughtItems,
            Status = ProviderBuyResponse.StatusEnum.Failed,
        };
        var requestedAmount = buyRequest.RequestedAssets.Sum(x => x.Quantity);
        var fulfilledAmount = boughtItems.Sum(x => x.Quantity);

        if (requestedAmount == fulfilledAmount)
        {
            response.Status = ProviderBuyResponse.StatusEnum.Fulfilled;
        }
        else if (fulfilledAmount > 0)
        {
            response.Status = ProviderBuyResponse.StatusEnum.Partial;
        }

        //Send buy response first to unblock end user,
        eventSender.SendBuyResponseEvent(response);

        SaveInventoryItems(affectedInventoryItems);
    }

    private void SaveInventoryItem(InternalInventoryItemDb dbItem)
    {
        eventSender.SendInternalInventoryItemChangeEvent(Map(dbItem));
        internalInventoryItemRepository.Add(dbItem);
    }

    private void SaveInventoryItems(IList<InternalInventoryItemDb> dbItems)
    {
        foreach (var dbItem in dbItems)
        {
            eventSender.SendInternalInventoryItemChangeEvent(Map(dbItem));
        }

        internalInventoryItemRepository.Add(dbItems);
    }

    public void ClearCache()
    {
        itemsByID.Clear();
        itemsBySymbol.Clear();
        itemsBySource.Clear();
        internalInventoryItemRepository.DeleteAll();
    }

    private static InternalInventoryItem Map(InternalInventoryItemDb dbItem)
    {
        return new InternalInventoryItem()
        {
            Id = dbItem.Id,
            Price = dbItem.Price,
            Quantity = dbItem.Quantity,
            Source = dbItem.Source,
            Status = dbItem.Status,
            Symbol = dbItem.Symbol,
            Tag = dbItem.Tag,
            CoveredInvItemId = dbItem.CoveredInvItemId,
            Version = dbItem.Version,
            CreatingType = dbItem.CreatingType,
            CreatedAt = dbItem.CreatedAt,
            SoldQuantity = dbItem.SoldQuantity,
        };
    }

    private static InternalInventoryItemDb Map(InternalInventoryItem item)
    {
        return new InternalInventoryItemDb()
        {
            Id = item.Id,
            Price = item.Price,
            Quantity = item.Quantity,
            Source = item.Source,
            Status = item.Status,
            Symbol = item.Symbol,
            Tag = item.Tag,
            CoveredInvItemId = item.CoveredInvItemId,
            Version = item.Version,
            CreatingType = item.CreatingType,
            CreatedAt = item.CreatedAt,
            SoldQuantity = item.SoldQuantity,
        };
    }
}
