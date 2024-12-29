using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Polly;
using InternalInventory.API.Services.Interfaces;

namespace InternalInventory.API.HostedServices;

public class DayDataCleaner : IHostedService, IDisposable
{
    private readonly IDataCleaner _dataCleaner;
    private readonly ITimeService _timeService;

    private Timer? _timer;

    private readonly Policy _retryPolicy = Policy.Handle<Exception>()
        .WaitAndRetry(retryCount: 1000, sleepDurationProvider: _ => TimeSpan.FromSeconds(3),
            onRetry: (exception, sleepDuration, attemptNumber, context) =>
            {
            });

    public DayDataCleaner(IDataCleaner dataCleaner,
        ITimeService timeService)
    {
        _dataCleaner = dataCleaner;
        _timeService = timeService;
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
            _dataCleaner.CleanData();
        });

        if (result.FinalException == null) return;

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