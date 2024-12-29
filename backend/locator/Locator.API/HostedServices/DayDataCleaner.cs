using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Polly;
using Locator.API.Services.Interfaces;

namespace Locator.API.HostedServices;

public class DayDataCleaner : IHostedService, IDisposable
{
    private readonly ITimeService _timeService;
    private readonly IDataCleaner _dataCleaner;

    private Timer? _timer;

    private readonly Policy _retryPolicy = Policy.Handle<Exception>()
        .WaitAndRetry(retryCount: 1000, sleepDurationProvider: _ => TimeSpan.FromSeconds(3),
            onRetry: (exception, sleepDuration, attemptNumber, context) =>
            {
            });

    public DayDataCleaner(ITimeService timeService,
        IDataCleaner dataCleaner)
    {
        _timeService = timeService;
        _dataCleaner = dataCleaner;
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
        using var activity = TracingConfiguration.StartActivity("DayDataCleaner CleanData");
        try
        {
            var result = _retryPolicy.ExecuteAndCapture(() =>
            {
                _dataCleaner.CleanData();
            });

            if (result.FinalException == null) return;
        }
        catch (Exception ex)
        {
            activity.LogException(ex);
            throw;
        }

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