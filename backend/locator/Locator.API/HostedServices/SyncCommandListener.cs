using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Locator.API.Models.Options;
using Locator.API.Services.Interfaces;
using Locator.API.Storages.Interfaces;
using Shared;

namespace Locator.API.HostedServices;

public class SyncCommandListener : BackgroundService
{
    private readonly IProviderSettingStorage _providerSettingStorage;
    private readonly IAutoDisableProvidersService _autoDisableProvidersService;

    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    public SyncCommandListener(IProviderSettingStorage providerSettingStorage,
        IOptions<KafkaOptions> kafkaOptions, IAutoDisableProvidersService autoDisableProvidersService)
    {
        _providerSettingStorage = providerSettingStorage;
        _autoDisableProvidersService = autoDisableProvidersService;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = Environment.MachineName + kafkaOptions.Value.GroupId,
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
        using var activity = TracingConfiguration.StartActivity("SyncCommandListener ProcessMessage");
        try
        {
            var message = consumeResult.Message.Value;

            Action action = message switch
            {
                SyncCommand.InvalidateCache(SyncCommand.CacheTypeEnum.ProviderSettings) => () => _providerSettingStorage.RefreshStorage(),
                SyncCommand.EnableProvider(string ProviderId) => () => _autoDisableProvidersService.EnableProviderBack(ProviderId),
                _ => () => { throw new NotImplementedException(); }
                ,
            };
            action();
        }
        catch (Exception ex)
        {
            activity.LogException(ex);
        }
    }
}