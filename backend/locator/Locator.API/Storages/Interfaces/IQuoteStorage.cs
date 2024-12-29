using Locator.API.Models.Internal;

namespace Locator.API.Storages.Interfaces;

public interface IQuoteStorage
{
    bool TryAddQuoteWithLock(Quote quote);
    Quote? GetQuoteWithLock(Quote.QuoteKey key);
    Quote? ReleaseQuote(Quote.QuoteKey id);
    Quote? TryGetQuoteWithLock(Quote.QuoteKey id);

    Quote[] GetQuotes(Quote.QuoteStatus status, DateTime updatedBefore);
    Quote[] GetNotActiveQuotes(DateTime updatedBefore);
    void DeleteQuote(Quote quote);

    void ClearCache();
}