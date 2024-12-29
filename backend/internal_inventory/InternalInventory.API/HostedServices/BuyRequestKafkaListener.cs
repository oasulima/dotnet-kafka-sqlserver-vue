using Confluent.Kafka;
using Microsoft.Extensions.Options;
using InternalInventory.API.Models.Options;
using InternalInventory.API.Services.Interfaces;
using Shared;

namespace InternalInventory.API.HostedServices;

public class BuyRequestKafkaListener : BackgroundService
{
    private readonly IInventoryService _service;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;
    public BuyRequestKafkaListener(IInventoryService service, IOptions<KafkaOptions> kafkaOptions)
    {
        _service = service;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest
        };
        _topic = kafkaOptions.Value.BuyRequestTopic;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<string, ProviderBuyRequest>(_consumerConfig).SetValueDeserializer(new KafkaDeserializer<ProviderBuyRequest>()).Build();
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

    private void ProcessMessage(ConsumeResult<string, ProviderBuyRequest> consumeResult)
    {
        var message = consumeResult.Message.Value;

        _service.ProcessBuyRequest(message);
    }
}