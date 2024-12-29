using System.Collections.Concurrent;
using System.Linq;
using Admin.API.Models.Cache;
using Admin.API.Services.Interfaces;
using Shared.Quote;

namespace Admin.API.Services;

public class LocatesCache : ILocatesCache
{
    private readonly ConcurrentBag<LocateModel> _locatesCache = new();

    public LocateModel[] GetHistoryRecords()
    {
        return _locatesCache.AsEnumerable().OrderByDescending(x => x.Time).ToArray();
    }

    public LocateModel Memorize(LocatorQuoteResponse message)
    {
        var entity = new LocateModel
        {
            QuoteId = message.Id,
            Time = message.Time,
            AccountId = message.AccountId,
            Symbol = message.Symbol,
            QtyFill = message.FillQty ?? 0,
            ReqQty = message.ReqQty,
            Price = message.Price ?? 0,
            DiscountedPrice = message.DiscountedPrice ?? 0,
            Status = message.Status,
            ErrorMessage = message.ErrorMessage,
            Source = message.Source,
            SourceDetails = message.Sources.ToArray()
        };

        _locatesCache.Add(entity);
        return entity;
    }

    public void Clear()
    {
        _locatesCache.Clear();
    }
}