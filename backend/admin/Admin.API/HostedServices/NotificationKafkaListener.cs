using Admin.API.Options;
using Admin.API.Services.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Shared;

namespace Admin.API.HostedServices;

public class NotificationKafkaListener : BackgroundService
{
    private readonly INotificationsServiceBase _adminUiHubMethods;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    public NotificationKafkaListener(
        IOptions<KafkaOptions> kafkaOptions,
        INotificationsServiceBase adminUiHubMethods
    )
    {
        _adminUiHubMethods = adminUiHubMethods;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest,
        };
        _topic = kafkaOptions.Value.NotificationTopic;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<string, GroupedNotification[]>(_consumerConfig)
                .SetValueDeserializer(new KafkaDeserializer<GroupedNotification[]>())
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

    private void ProcessMessage(ConsumeResult<string, GroupedNotification[]> consumeResult)
    {
        var message = consumeResult.Message.Value;

        _adminUiHubMethods.SendNotifications(message);
    }
}
