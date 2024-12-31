using Locator.API.Services.Interfaces;

namespace Locator.API.HostedServices;

public class QuoteTimeoutProcessor : BackgroundService
{
    private readonly ILocatorService _locatorService;
    private readonly TimeSpan _delayPeriod = TimeSpan.FromMilliseconds(500);

    public QuoteTimeoutProcessor(ILocatorService locatorService)
    {
        _locatorService = locatorService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _locatorService.ProcessHangingQuotes();
                await Task.Delay(_delayPeriod, stoppingToken);
            }
            catch (Exception e)
            {
                var activity = TracingConfiguration.StartActivity("QuoteTimeoutProcessor ExecuteAsync");
                activity.LogException(e);
            }
        }
    }
}