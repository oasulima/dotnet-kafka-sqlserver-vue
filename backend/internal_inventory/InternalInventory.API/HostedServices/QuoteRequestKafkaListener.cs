using Confluent.Kafka;
using Microsoft.Extensions.Options;
using InternalInventory.API.Models.Options;
using InternalInventory.API.Services.Interfaces;
using Shared;

namespace InternalInventory.API.HostedServices;

public class QuoteRequestKafkaListener : BackgroundService
{
    private readonly IInventoryService _inventoryService;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    public QuoteRequestKafkaListener(IInventoryService inventoryService, IOptions<KafkaOptions> kafkaOptions)
    {
        _inventoryService = inventoryService;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest
        };
        _topic = kafkaOptions.Value.QuoteRequestTopic;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<string, ProviderQuoteRequest>(_consumerConfig).SetValueDeserializer(new KafkaDeserializer<ProviderQuoteRequest>()).Build();
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

    private void ProcessMessage(ConsumeResult<string, ProviderQuoteRequest> consumeResult)
    {
        var message = consumeResult.Message.Value;
        _inventoryService.ProcessQuoteRequest(message);
    }
}