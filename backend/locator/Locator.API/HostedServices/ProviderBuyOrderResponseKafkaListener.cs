using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Shared;
using Locator.API.Models.Options;
using Locator.API.Services.Interfaces;
using Shared;
using Shared.Settings;

namespace Locator.API.HostedServices;

public class ProviderBuyOrderResponseKafkaListener : BackgroundService
{
    private readonly ILocatorService _locatorService;
    private readonly ISettingService _settingService;
    private readonly Dictionary<string, (Task Task, CancellationTokenSource CTS)> _listeners = new();

    private readonly TimeSpan _delayForCheckingTopicUpdates = TimeSpan.FromSeconds(5);
    private readonly ConsumerConfig _consumerConfig;

    public ProviderBuyOrderResponseKafkaListener(IOptions<KafkaOptions> kafkaOptions,
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
        using var activity = TracingConfiguration.StartActivity("ProviderBuyOrderResponseKafkaListener AddMissingListeners");

        ProviderSetting[] providers = _settingService.GetActiveExternalProviderSettings();
        foreach (var provider in providers)
        {
            if (provider.BuyResponseTopic == null)
            {
                continue;
            }


            if (!_listeners.ContainsKey(provider.BuyResponseTopic) ||
                _listeners[provider.BuyResponseTopic].Task.IsCompleted)
            {
                if (_listeners.ContainsKey(provider.BuyResponseTopic))
                {
                    activity.SetTag("topic", provider.BuyResponseTopic);
                    activity.SetTag("status", _listeners[provider.BuyResponseTopic].Task.Status);
                }
                _listeners[provider.BuyResponseTopic] = StartProviderListener(provider, stoppingToken);
            }
        }

        var topicsToRemove = _listeners.Keys.Except(providers.Select(x => x.BuyResponseTopic)).ToArray();

        activity.SetTag("topicsToRemove", string.Join(',', topicsToRemove));


        foreach (var topicToRemove in topicsToRemove)
        {
            //TopicsToRemove cannot be null, it is NotNullable.Except(Nullable), in results it will always be not nullable

            _listeners[topicToRemove!].CTS.Cancel();
            //We don't need to remove the task from the dictionary, it will be in completed state
        }
    }

    private (Task, CancellationTokenSource) StartProviderListener(ProviderSetting provider,
        CancellationToken cancellationToken)
    {
        using var activity = TracingConfiguration.StartActivity("ProviderBuyOrderResponseKafkaListener StartProviderListener");
        var topic = provider.BuyResponseTopic;
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        activity.SetTag("topic", topic);
        activity.SetTag("cts", cts.Token.GetHashCode());
        try
        {

            var task = Task.Run(
                async () =>
                {
                    using var consumer = new ConsumerBuilder<string, ProviderBuyResponse>(_consumerConfig).SetValueDeserializer(new KafkaDeserializer<ProviderBuyResponse>()).Build();
                    consumer.Subscribe(topic);
                    Console.WriteLine($"subscribe to {topic}");
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
                        Console.WriteLine($"close consumer to {topic}");
                        consumer.Close();
                    }
                },
                cancellationToken);
            return (task, cts);
        }
        catch (Exception ex)
        {
            activity.LogException(ex);
            return (Task.CompletedTask, cts);
        }
    }

    private void ProcessMessage(ProviderSetting provider, ConsumeResult<string, ProviderBuyResponse> consumeResult)
    {
        using var activity = TracingConfiguration.StartActivity("ProviderBuyOrderResponseKafkaListener ProcessMessage");
        try
        {
            var message = consumeResult.Message.Value;

            _locatorService.ProcessProviderBuyOrderResponse(message, provider.ProviderId.ToLower());
        }
        catch (Exception ex)
        {
            activity.LogException(ex);
        }
    }
}