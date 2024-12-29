using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Reporting.API.Data.Repositories.Interfaces;
using Reporting.API.Models.Options;
using Reporting.API.Utility;
using Shared;

namespace Reporting.API.HostedServices;

public class QuoteRequestKafkaListener : BackgroundService
{
    private readonly IQuoteRequestRepository quoteRequestRepository;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    public QuoteRequestKafkaListener(
        IOptions<KafkaOptions> kafkaOptions,
        IQuoteRequestRepository quoteRequestRepository)
    {
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest
        };
        _topic = kafkaOptions.Value.QuoteRequestTopic;
        this.quoteRequestRepository = quoteRequestRepository;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<string, QuoteRequest>(_consumerConfig).SetValueDeserializer(new KafkaDeserializer<QuoteRequest>()).Build();
            consumer.Subscribe(_topic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await consumer.ConsumeWithTracing((result) =>
                    {
                        ProcessMessage(result, quoteRequestRepository);
                    }, stoppingToken);
                }
            }
            finally
            {
                consumer.Close();
            }
        });
    }

    private void ProcessMessage(ConsumeResult<string, QuoteRequest> consumeResult, IQuoteRequestRepository quoteRequestRepository)
    {
        var quoteRequest = consumeResult.Message.Value;
        quoteRequestRepository.AddQuoteRequest(quoteRequest.ToQuoteRequestDb());
    }
}