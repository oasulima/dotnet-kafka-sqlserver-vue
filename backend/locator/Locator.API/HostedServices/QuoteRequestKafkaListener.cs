using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Locator.API.Models.Options;
using Locator.API.Services.Interfaces;
using Shared;

namespace Locator.API.HostedServices;

public class QuoteRequestKafkaListener : BackgroundService
{
    private readonly ILocatorService _locatorService;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    public QuoteRequestKafkaListener(IOptions<KafkaOptions> kafkaOptions,
        ILocatorService locatorService)
    {
        _locatorService = locatorService;
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
            using var consumer = new ConsumerBuilder<string, QuoteRequest>(_consumerConfig).SetValueDeserializer(new KafkaDeserializer<QuoteRequest>()).Build();
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

    private void ProcessMessage(ConsumeResult<string, QuoteRequest> consumeResult)
    {
        using var activity = TracingConfiguration.StartActivity("QuoteRequestKafkaListener ProcessMessage");
        try
        {
            var timeRequestArrived = DateTime.UtcNow;


            var message = consumeResult.Message.Value;


            switch (message.RequestType)
            {
                case QuoteRequest.RequestTypeEnum.QuoteRequest:
                    _locatorService.CreateQuote(message, timeRequestArrived);
                    break;
                case QuoteRequest.RequestTypeEnum.QuoteAccept:
                    _locatorService.AcceptQuote(message);
                    break;
                case QuoteRequest.RequestTypeEnum.QuoteCancel:
                    _locatorService.CancelQuote(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception ex)
        {
            activity.LogException(ex);
        }
    }
}