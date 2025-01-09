using InternalInventory.API.Models.Options;
using Microsoft.Extensions.Options;
using Polly;
using Shared;
using Shared.Refit;
using Shared.Settings;

namespace InternalInventory.API.HostedServices;

public class SelfRegHostedServices : BackgroundService
{
    private readonly ILocatorApi _locatorApi;
    private readonly IOptions<SelfOptions> _selfOptions;
    private readonly IOptions<KafkaOptions> _kafkaOptions;

    private AsyncPolicy<ProviderSetting> _policy = Policy
        .HandleResult((ProviderSetting response) => response == null)
        .Or<Exception>()
        .WaitAndRetryAsync(
            int.MaxValue,
            _ => TimeSpan.FromSeconds(10),
            (dr, x1, x2) => LogNotSuccess(dr)
        );

    public SelfRegHostedServices(
        IOptions<SelfOptions> selfOptions,
        IOptions<KafkaOptions> kafkaOptions,
        ILocatorApi locatorApi
    )
    {
        _selfOptions = selfOptions;
        _kafkaOptions = kafkaOptions;
        _locatorApi = locatorApi;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var body = new ProviderSelfRegRequest()
        {
            Id = _selfOptions.Value.Id,
            Name = _selfOptions.Value.Name,
            BuyRequestTopic = _kafkaOptions.Value.BuyRequestTopic,
            BuyResponseTopic = _kafkaOptions.Value.OrderReportTopic,
            QuoteRequestTopic = _kafkaOptions.Value.QuoteRequestTopic,
            QuoteResponseTopic = _kafkaOptions.Value.QuoteResponseTopic,
        };
        await _policy.ExecuteAsync(() => _locatorApi.SelfReg(body));
    }

    private static void LogNotSuccess(DelegateResult<ProviderSetting> delegateResult) { }
}
