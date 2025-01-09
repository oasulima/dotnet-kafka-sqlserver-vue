using System.Collections.Concurrent;
using Locator.API.Models.Options;
using Locator.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Locator;
using TProviderId = System.String;
using TSymbol = System.String;

namespace Locator.API.Services;

public class AutoDisableProvidersService : IAutoDisableProvidersService
{
    private readonly IOptions<AutoDisableOptions> _options;
    private readonly INotificationService _notificationService;
    private int MinFailed => _options.Value.MinFailed;
    private double PercentOfFailed => _options.Value.PercentOfFailed;
    private TimeSpan SlidingWindow => _options.Value.SlidingWindow;
    private bool TakeQuoteSuccessIntoAccount => _options.Value.TakeQuoteSuccessIntoAccount;

    private record SymbolProvider(string Provider, string Symbol);

    private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _successesByProvider =
        new();
    private readonly ConcurrentDictionary<
        SymbolProvider,
        ConcurrentQueue<DateTime>
    > _successesBySymbolProvider = new();
    private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _failsByProvider =
        new();
    private readonly ConcurrentDictionary<
        SymbolProvider,
        ConcurrentQueue<DateTime>
    > _failsBySymbolProvider = new();

    private readonly ConcurrentDictionary<string, bool> _isDisabledByProvider = new();
    private readonly ConcurrentDictionary<SymbolProvider, bool> _isDisabledBySymbolProvider = new();

    public AutoDisableProvidersService(
        IOptions<AutoDisableOptions> options,
        INotificationService notificationService
    )
    {
        _options = options;
        _notificationService = notificationService;
    }

    public void RegisterProviderQuote(string provider, string symbol, bool isAnswered)
    {
        if (TakeQuoteSuccessIntoAccount || !isAnswered)
        {
            RegisterInternal(provider, symbol, isAnswered);
        }
    }

    private void RegisterInternal(string provider, string symbol, bool isAnswered)
    {
        var time = DateTime.UtcNow;
        var dictionary = isAnswered ? _successesByProvider : _failsByProvider;
        var providerQueue = dictionary.GetOrAdd(provider, _ => new());
        providerQueue.Enqueue(time);

        var symbolDictionary = isAnswered ? _successesBySymbolProvider : _failsBySymbolProvider;
        var symbolProvider = new SymbolProvider(provider, symbol);
        var symbolProviderQueue = symbolDictionary.GetOrAdd(symbolProvider, _ => new());
        symbolProviderQueue.Enqueue(time);

        if (!isAnswered && !_isDisabledByProvider.GetValueOrDefault(provider))
        {
            CalculateStatus(provider);
        }

        if (
            !isAnswered
            && !_isDisabledByProvider.GetValueOrDefault(provider)
            && !_isDisabledBySymbolProvider.GetValueOrDefault(symbolProvider)
        )
        {
            CalculateStatus(symbolProvider);
        }
    }

    public void RegisterProviderBuy(string provider, string symbol, bool isAnswered)
    {
        RegisterInternal(provider, symbol, isAnswered);
    }

    public bool IsProviderDisabled(string provider, string symbol)
    {
        return _isDisabledByProvider.GetValueOrDefault(provider)
            || _isDisabledBySymbolProvider.GetValueOrDefault(new SymbolProvider(provider, symbol));
    }

    public Dictionary<TProviderId, TSymbol[]?> GetDisabledProviders()
    {
        var result = _isDisabledByProvider
            .Select(x => x.Key)
            .ToDictionary(x => x, x => (TSymbol[]?)null);

        var ps = _isDisabledBySymbolProvider
            .Select(x => x.Key)
            .GroupBy(x => x.Provider, x => x.Symbol)
            .ToDictionary(x => x.Key, x => x.ToArray());
        foreach (var provider in ps.Keys)
        {
            if (!result.ContainsKey(provider))
            {
                result.Add(provider, ps[provider]);
            }
        }

        return result;
    }

    public void EnableProviderBack(string provider)
    {
        _isDisabledByProvider.TryRemove(provider, out _);
        CleanSymbolProviderQueue(provider, _isDisabledBySymbolProvider);

        _successesByProvider.TryRemove(provider, out _);
        CleanSymbolProviderQueue(provider, _successesBySymbolProvider);
        _failsByProvider.TryRemove(provider, out _);
        CleanSymbolProviderQueue(provider, _failsBySymbolProvider);

        _notificationService.Add(
            new NotificationEvent(
                Type: NotificationType.Warning,
                Kind: LocatorErrorKind.ProviderReEnabled.ToString(),
                GroupParameters: provider,
                Time: DateTime.UtcNow,
                Message: $"Provider {provider} is re-enabled by admin"
            )
        );
    }

    public void Clear()
    {
        _successesByProvider.Clear();
        _failsByProvider.Clear();
        _failsBySymbolProvider.Clear();
        _successesBySymbolProvider.Clear();

        _isDisabledBySymbolProvider.Clear();
        _isDisabledByProvider.Clear();
    }

    private static void CleanSymbolProviderQueue<T>(
        string provider,
        ConcurrentDictionary<SymbolProvider, T> isDisabledBySymbolProvider
    )
    {
        var symbolProviders = isDisabledBySymbolProvider
            .Select(x => x.Key)
            .Where(x => string.Equals(x.Provider, provider, StringComparison.OrdinalIgnoreCase));
        foreach (var symbolProvider in symbolProviders)
        {
            isDisabledBySymbolProvider.TryRemove(symbolProvider, out _);
        }
    }

    private void CalculateStatus(string provider)
    {
        var successQueue = _successesByProvider.GetOrAdd(provider, _ => new());
        var failedQueue = _failsByProvider.GetOrAdd(provider, _ => new());

        var newStatus = CalculateStatus(successQueue, failedQueue);

        if (newStatus)
        {
            _isDisabledByProvider.TryAdd(provider, newStatus);
            var message =
                $"Provider {provider} is not answering. It is disabled till the end of the day";
            _notificationService.Add(
                new NotificationEvent(
                    Type: NotificationType.Warning,
                    Kind: LocatorErrorKind.ProviderAutoDisabled.ToString(),
                    GroupParameters: provider,
                    Time: DateTime.UtcNow,
                    Message: message
                )
            );
        }
    }

    private void CalculateStatus(SymbolProvider symbolProvider)
    {
        var successQueue = _successesBySymbolProvider.GetOrAdd(symbolProvider, _ => new());
        var failedQueue = _failsBySymbolProvider.GetOrAdd(symbolProvider, _ => new());

        var newStatus = CalculateStatus(successQueue, failedQueue);

        if (newStatus)
        {
            _isDisabledBySymbolProvider.TryAdd(symbolProvider, newStatus);
            var message =
                $"Provider {symbolProvider.Provider} is not answering for symbol {symbolProvider.Symbol}. "
                + $"Symbol {symbolProvider.Symbol} is disable till end of the day";
            _notificationService.Add(
                new NotificationEvent(
                    Type: NotificationType.Warning,
                    Kind: LocatorErrorKind.SymbolInProviderAutoDisabled.ToString(),
                    GroupParameters: $"{symbolProvider.Provider}_{symbolProvider.Symbol}",
                    Time: DateTime.UtcNow,
                    Message: message
                )
            );
        }
    }

    private bool CalculateStatus(
        ConcurrentQueue<DateTime> successQueue,
        ConcurrentQueue<DateTime> failedQueue
    )
    {
        var startFrom = DateTime.UtcNow - SlidingWindow;

        RemoveObsoleteItems(successQueue, startFrom);
        RemoveObsoleteItems(failedQueue, startFrom);

        var successCount = successQueue.Count;
        var failedCount = failedQueue.Count;
        var percent = (double)failedCount / (successCount + failedCount);
        return failedCount >= MinFailed && percent >= PercentOfFailed;
    }

    private static void RemoveObsoleteItems(ConcurrentQueue<DateTime> queue, DateTime startFrom)
    {
        while (queue.TryPeek(out DateTime item) && item < startFrom)
        {
            //We can loose something here because of multithreading, but I don't think it is critical
            queue.TryDequeue(out _);
        }
    }
}
