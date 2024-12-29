using Admin.API.Models.Cache;
using Shared.Quote;

namespace Admin.API.Services.Interfaces;

public interface ILocateRequestsCache
{
    LocateRequestModel[] GetHistoryRecords();
    LocateRequestModel Memorize(LocatorQuoteResponse quoteResponse);
    void Clear();
}