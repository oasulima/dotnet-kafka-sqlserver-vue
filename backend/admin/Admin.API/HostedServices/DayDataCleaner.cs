using Admin.API.Services.Interfaces;
using Polly;

namespace Admin.API.HostedServices;

public class DayDataCleaner : IHostedService, IDisposable
{
    private readonly ITimeService _timeService;
    private readonly IMessageHandler _messageHandler;

    private Timer? _timer;

    private readonly Policy _retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetry(
            retryCount: 1000,
            sleepDurationProvider: _ => TimeSpan.FromSeconds(3),
            onRetry: (exception, sleepDuration, attemptNumber, context) => { }
        );

    public DayDataCleaner(ITimeService timeService, IMessageHandler messageHandler)
    {
        _timeService = timeService;
        _messageHandler = messageHandler;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        using var activity = TracingConfiguration.StartActivity("DayDataCleaner StartAsync");
        try
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
        catch (Exception ex)
        {
            activity?.LogException(ex);
            throw;
        }
    }

    private void CleanData(object? obj)
    {
        using var activity = TracingConfiguration.StartActivity("DayDataCleaner CleanData");
        try
        {
            var result = _retryPolicy.ExecuteAndCapture(() =>
            {
                _messageHandler.CleanData();
            });

            if (result.FinalException == null)
            {
                return;
            }
        }
        catch (Exception ex)
        {
            activity?.LogException(ex);
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
