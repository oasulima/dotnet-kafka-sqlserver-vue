using System;
using System.Collections.Concurrent;
using System.Linq;
using Admin.API.Models;
using Admin.API.Models.Cache;
using Admin.API.Services.Interfaces;
using Shared;
using Shared.Quote;

namespace Admin.API.Services;

public class LocateRequestsCache : ILocateRequestsCache
{
    private readonly ConcurrentDictionary<string, LocateRequestModel> _locateRequestsCache = new();

    public LocateRequestModel[] GetHistoryRecords()
    {
        return _locateRequestsCache.Select(x => x.Value).OrderByDescending(x => x.Time).ToArray();
    }

    public void Clear()
    {
        _locateRequestsCache.Clear();
    }

    public LocateRequestModel Memorize(LocatorQuoteResponse message)
    {
        var entity = GetEntity(message.Id, message.Time);

        entity.AccountId = message.AccountId;
        entity.Symbol = message.Symbol;
        entity.QtyReq = message.ReqQty;
        entity.QtyOffer = message.FillQty ?? 0;
        entity.Price = message.Price ?? 0;
        entity.DiscountedPrice = message.DiscountedPrice ?? 0;
        entity.Source = message.Sources != null
            ? string.Join(", ", message.Sources.Select(x => x.Source).Distinct())
            : string.Empty;
        entity.SourceDetails = message.Sources?.ToArray() ?? Array.Empty<QuoteSourceInfo>();

        return entity;
    }

    private LocateRequestModel GetEntity(string id, DateTime time)
    {
        var lazy = new Lazy<LocateRequestModel>(() => new LocateRequestModel { Id = id, Time = time });
        return _locateRequestsCache.GetOrAdd(id, _ => lazy.Value);
    }
}