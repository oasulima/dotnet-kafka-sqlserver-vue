using InternalInventory.API.Services.Interfaces;
using Polly;

namespace InternalInventory.API.HostedServices;

public class DayDataCleaner : IHostedService, IDisposable
{
    private readonly IInventoryService inventoryService;
    private readonly ITimeService _timeService;

    private Timer? _timer;

    private readonly Policy _retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetry(
            retryCount: 1000,
            sleepDurationProvider: _ => TimeSpan.FromSeconds(3),
            onRetry: (exception, sleepDuration, attemptNumber, context) => { }
        );

    public DayDataCleaner(ITimeService timeService, IInventoryService inventoryService)
    {
        _timeService = timeService;
        this.inventoryService = inventoryService;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        var cleanupDelay = _timeService.GetNextCleanupDelay(DateTime.UtcNow);

        if (_timer != null)
        {
            _timer.Change(cleanupDelay, TimeSpan.FromDays(1));
        }
        else
        {
            _timer = new Timer(CleanData, null, cleanupDelay, TimeSpan.FromDays(1));
        }

        return Task.CompletedTask;
    }

    private void CleanData(object? obj)
    {
        var result = _retryPolicy.ExecuteAndCapture(() =>
        {
            inventoryService.ClearCache();
        });

        if (result.FinalException == null)
            return;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
