using Admin.API.Models.Cache;
using Shared.Quote;

namespace Admin.API.Services.Interfaces;

public interface ILocatesCache
{
    LocateModel[] GetHistoryRecords();
    LocateModel Memorize(LocatorQuoteResponse quoteResponse);
    void Clear();
}