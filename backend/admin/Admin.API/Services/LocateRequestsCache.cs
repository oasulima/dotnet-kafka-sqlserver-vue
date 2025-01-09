using System.Collections.Concurrent;
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
        var model = new LocateRequestModel
        {
            Id = message.Id,
            AccountId = message.AccountId,
            Symbol = message.Symbol,
            QtyReq = message.ReqQty,
            QtyOffer = message.FillQty ?? 0,
            Time = message.Time,
            Price = message.Price ?? 0,
            DiscountedPrice = message.DiscountedPrice ?? 0,
            Source =
                message.Sources != null
                    ? string.Join(", ", message.Sources.Select(x => x.Source).Distinct())
                    : string.Empty,
            SourceDetails = message.Sources  ?? Array.Empty<QuoteSourceInfo>(),
        };

        var entity = _locateRequestsCache.AddOrUpdate(
            message.Id,
            model,
            (key, oldValue) =>
            {
                oldValue.AccountId = model.AccountId;
                oldValue.Symbol = model.Symbol;
                oldValue.QtyReq = model.QtyReq;
                oldValue.QtyOffer = model.QtyOffer;
                oldValue.Time = model.Time;
                oldValue.Price = model.Price;
                oldValue.DiscountedPrice = model.DiscountedPrice;
                oldValue.Source = model.Source;
                oldValue.SourceDetails = model.SourceDetails;
                return oldValue;
            }
        );

        return entity;
    }
}
