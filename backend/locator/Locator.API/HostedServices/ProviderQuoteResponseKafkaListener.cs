using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Locator.API.Models.Options;
using Locator.API.Services.Interfaces;
using Shared;
using Shared.Settings;

namespace Locator.API.HostedServices;

public class ProviderQuoteResponseKafkaListener : BackgroundService
{
    private readonly ILocatorService _locatorService;
    private readonly ISettingService _settingService;
    private readonly Dictionary<string, (Task Task, CancellationTokenSource CTS)> _listeners = new();

    private readonly TimeSpan _delayForCheckingTopicUpdates = TimeSpan.FromSeconds(5);
    private readonly ConsumerConfig _consumerConfig;

    public ProviderQuoteResponseKafkaListener(IOptions<KafkaOptions> kafkaOptions,
        ILocatorService locatorService,
        ISettingService settingService)
    {
        _locatorService = locatorService;
        _settingService = settingService;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            AddMissingListeners(stoppingToken);
            await Task.Delay(_delayForCheckingTopicUpdates, stoppingToken);
        }
    }

    private void AddMissingListeners(CancellationToken stoppingToken)
    {
        var providers = _settingService.GetActiveExternalProviderSettings();
        foreach (var provider in providers)
        {
            if (provider.QuoteResponseTopic == null)
            {
                continue;
            }

            if (!_listeners.ContainsKey(provider.QuoteResponseTopic) ||
                _listeners[provider.QuoteResponseTopic].Task.IsCompleted)
            {
                _listeners[provider.QuoteResponseTopic] = StartProviderListener(provider, stoppingToken);
            }
        }

        foreach (var topicToRemove in _listeners.Keys.Except(providers.Select(x => x.QuoteResponseTopic)))
        {
            //TopicsToRemove cannot be null, it is NotNullable.Except(Nullable), in results it will always be not nullable

            _listeners[topicToRemove!].CTS.Cancel();
            //We don't need to remove the task from the dictionary, it will be in completed state
        }
    }

    private (Task, CancellationTokenSource) StartProviderListener(ProviderSetting provider,
        CancellationToken cancellationToken)
    {
        var topic = provider.QuoteResponseTopic;
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var task = Task.Run(
            async () =>
            {
                using var consumer = new ConsumerBuilder<string, ProviderQuoteResponse>(_consumerConfig).SetValueDeserializer(new KafkaDeserializer<ProviderQuoteResponse>()).Build();
                consumer.Subscribe(topic);
                try
                {
                    while (!cts.IsCancellationRequested)
                    {
                        await consumer.ConsumeWithTracing((result) =>
                        {
                            ProcessMessage(provider, result);
                        }, cts.Token);
                    }
                }
                finally
                {
                    consumer.Close();
                }
            },
            cancellationToken);
        return (task, cts);
    }

    private void ProcessMessage(ProviderSetting provider,
        ConsumeResult<string, ProviderQuoteResponse> consumeResult)
    {
        using var activity = TracingConfiguration.StartActivity("ProviderQuoteResponseKafkaListener ProcessMessage");
        try
        {
            var message = consumeResult.Message.Value;
            _locatorService.ProcessProviderQuoteResponse(message, provider.ProviderId.ToLower());
        }
        catch (Exception ex)
        {
            activity.LogException(ex);
        }
    }
}