using System.Collections.Concurrent;
using System.Threading;
using Locator.API.Models.Internal;
using Locator.API.Storages.Interfaces;

namespace Locator.API.Storages;

public class QuoteStorage : IQuoteStorage
{
    private ConcurrentDictionary<Quote.AccountIdentifier,
            ConcurrentDictionary<Quote.QuoteKey, Quote>>
        _quotesByAccount = new();

    public bool TryAddQuoteWithLock(Quote quote)
    {
        Monitor.Enter(quote);
        var accountKey = quote.Account;
        var key = quote.Key;
        var accountCollection =
            _quotesByAccount.GetOrAdd(accountKey, _ => new());
        var result = accountCollection.TryAdd(key, quote);
        if (!result)
        {
            Monitor.Exit(quote);
        }

        return result;
    }

    public Quote? GetQuoteWithLock(Quote.QuoteKey key)
    {
        Quote? quote = GetQuoteInternal(key);
        if (quote != null)
        {
            Monitor.Enter(quote);
        }

        return quote;
    }

    public Quote? ReleaseQuote(Quote.QuoteKey id)
    {
        var quote = GetQuoteInternal(id);
        if (quote != null && Monitor.IsEntered(quote))
        {
            try
            {
                Monitor.Exit(quote);
            }
            catch (SynchronizationLockException exception)
            {
            }
        }

        return quote;
    }

    public Quote? TryGetQuoteWithLock(Quote.QuoteKey id)
    {
        var quote = GetQuoteInternal(id);
        if (quote != null && Monitor.TryEnter(quote))
        {
            return quote;
        }

        return null;
    }

    public Quote[] GetQuotes(Quote.QuoteStatus status, DateTime updatedBefore)
    {
        return _quotesByAccount
            .SelectMany(qba => qba.Value.Select(q => q.Value))
            .Where(x => x.Status == status && x.LastStatusUpdate < updatedBefore)
            .ToArray();
    }

    public Quote[] GetNotActiveQuotes(DateTime updatedBefore)
    {
        return _quotesByAccount
            .SelectMany(qba => qba.Value.Select(q => q.Value))
            .Where(x => !x.IsActive && x.LastStatusUpdate < updatedBefore)
            .ToArray();
    }

    public void DeleteQuote(Quote quote)
    {
        if (_quotesByAccount.TryGetValue(quote.Account, out var quotes))
        {
            quotes.TryRemove(quote.Key, out var _);
        }
    }

    public void ClearCache()
    {
        _quotesByAccount.Clear();
    }

    private Quote? GetQuoteInternal(Quote.QuoteKey key)
    {
        var accountKey = new Quote.AccountIdentifier(key.AccountId);
        if (_quotesByAccount.TryGetValue(accountKey, out var quotes) &&
            quotes.TryGetValue(key, out var quote))
        {
            return quote;
        }

        return null;
    }
}