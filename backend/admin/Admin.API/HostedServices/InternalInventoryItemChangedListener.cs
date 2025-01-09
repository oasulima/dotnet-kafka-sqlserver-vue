using Admin.API.Options;
using Admin.API.Services.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Shared;

namespace Admin.API.HostedServices;

public class InternalInventoryItemChangedListener : BackgroundService
{
    private readonly INotificationsServiceBase _adminUiHubMethods;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    public InternalInventoryItemChangedListener(
        INotificationsServiceBase adminUiHubMethods,
        IOptions<KafkaOptions> kafkaOptions
    )
    {
        _adminUiHubMethods = adminUiHubMethods;

        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest,
        };
        _topic = kafkaOptions.Value.InternalInventoryItemChangeTopic;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<string, InternalInventoryItem>(_consumerConfig)
                .SetValueDeserializer(new KafkaDeserializer<InternalInventoryItem>())
                .Build();
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

    private void ProcessMessage(ConsumeResult<string, InternalInventoryItem> consumeResult)
    {
        var message = consumeResult.Message.Value;

        _adminUiHubMethods.SendInternalInventoryItem(message);
    }
}
