using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Reporting.API.Data.Repositories.Interfaces;
using Reporting.API.Models.Options;
using Reporting.API.Utility;
using Shared;

namespace Reporting.API.HostedServices;

public class InternalInventoryItemKafkaListener : BackgroundService
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    private readonly IInternalInventoryItemRepository internalInventoryItemRepository;

    public InternalInventoryItemKafkaListener(IOptions<KafkaOptions> kafkaOptions,
        IInternalInventoryItemRepository internalInventoryItemRepository)
    {
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest
        };
        _topic = kafkaOptions.Value.InternalInventoryItemReportingTopic;
        this.internalInventoryItemRepository = internalInventoryItemRepository;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<string, InternalInventoryItem>(_consumerConfig).SetValueDeserializer(new KafkaDeserializer<InternalInventoryItem>()).Build();
            consumer.Subscribe(_topic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await consumer.ConsumeWithTracing((result) =>
                    {
                        ProcessMessage(result, internalInventoryItemRepository);
                    }, stoppingToken);
                }
            }
            finally
            {
                consumer.Close();
            }
        });
    }

    private void ProcessMessage(ConsumeResult<string, InternalInventoryItem> consumeResult, IInternalInventoryItemRepository internalInventoryItemRepository)
    {
        var message = consumeResult.Message.Value;

        internalInventoryItemRepository.AddInternalInventoryItem(message.ToInternalInventoryItemDb());
    }
}