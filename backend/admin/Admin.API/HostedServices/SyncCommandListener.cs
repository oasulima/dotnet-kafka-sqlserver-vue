using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Admin.API.Options;
using Admin.API.Services.Interfaces;
using Shared;

namespace Admin.API.HostedServices;

public class SyncCommandListener : BackgroundService
{
    private readonly IProviderSettingCache _providerSettingCache;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    public SyncCommandListener(IOptions<KafkaOptions> kafkaOptions, IProviderSettingCache providerSettingCache)
    {
        _providerSettingCache = providerSettingCache;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest
        };
        _topic = kafkaOptions.Value.InvalidateCacheCommandTopic;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<string, SyncCommand>(_consumerConfig).SetValueDeserializer(new KafkaDeserializer<SyncCommand>()).Build();
            consumer.Subscribe(_topic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await consumer.ConsumeWithTracing(ProcessMessage, stoppingToken);
                }
            }
            finally
            {
                consumer.Close();
            }
        });
    }

    private void ProcessMessage(ConsumeResult<string, SyncCommand> consumeResult)
    {
        var message = consumeResult.Message.Value;
        if (message.CacheType == SyncCommand.CacheTypeEnum.ProviderSettings)
        {
            _providerSettingCache.RefreshSettings()
                .GetAwaiter()
                .GetResult(); //TODO: Can we use Kafka async listeners?
        }
    }
}