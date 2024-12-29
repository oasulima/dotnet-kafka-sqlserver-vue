using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
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
            using var activity = TracingConfiguration.StartActivity("QuoteTimeoutProcessor ExecuteAsync");
            try
            {
                _locatorService.ProcessHangingQuotes();
                await Task.Delay(_delayPeriod, stoppingToken);
            }
            catch (Exception e)
            {
                activity.LogException(e);
            }
        }
    }
}