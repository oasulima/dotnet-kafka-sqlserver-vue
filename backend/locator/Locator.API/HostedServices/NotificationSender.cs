using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Locator.API.Models.Options;
using Locator.API.Services.Interfaces;

namespace Locator.API.HostedServices;

public class NotificationSender : BackgroundService
{
    private readonly AppOptions _appOptions;
    private readonly INotificationService _service;

    public NotificationSender(INotificationService service, IOptions<AppOptions> appOptions)
    {
        _appOptions = appOptions.Value;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_appOptions.NotificationSenderInterval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var activity = TracingConfiguration.StartActivity("NotificationSender ExecuteAsync");
            try
            {
                _service.SendAll();
            }
            catch (Exception ex)
            {
                activity.LogException(ex);
            }
        }
    }
}