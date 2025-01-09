using Admin.API.Options;
using Admin.API.Services.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Shared;

namespace Admin.API.HostedServices;

public class LocatorQuoteResponseKafkaListener : BackgroundService
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;
    private readonly IMessageHandler _messageHandler;

    public LocatorQuoteResponseKafkaListener(
        IOptions<KafkaOptions> kafkaOptions,
        IMessageHandler messageHandler
    )
    {
        _messageHandler = messageHandler;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest,
        };
        _topic = kafkaOptions.Value.LocatorQuoteResponseTopic;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<string, QuoteResponse>(_consumerConfig)
                .SetValueDeserializer(new KafkaDeserializer<QuoteResponse>())
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

    private void ProcessMessage(ConsumeResult<string, QuoteResponse> consumeResult)
    {
        var message = consumeResult.Message.Value;

        _messageHandler.Handle(message);
    }
}
