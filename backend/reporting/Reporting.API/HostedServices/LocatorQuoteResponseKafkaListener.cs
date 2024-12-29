using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Reporting.API.Data.Repositories.Interfaces;
using Reporting.API.Models.Options;
using Reporting.API.Services.Interfaces;
using Shared;
using Shared.Quote;

namespace Reporting.API.HostedServices;

public class LocatorQuoteResponseKafkaListener : BackgroundService
{
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    private readonly ILocatesReportDataService locatesReportDataService;
    private readonly IQuoteDetailsRepository quoteDetailsRepository;

    public LocatorQuoteResponseKafkaListener(
        IOptions<KafkaOptions> kafkaOptions,
        ILocatesReportDataService locatesReportDataService,
        IQuoteDetailsRepository quoteDetailsRepository)
    {
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest
        };
        _topic = kafkaOptions.Value.QuoteResponseTopic;
        this.locatesReportDataService = locatesReportDataService;
        this.quoteDetailsRepository = quoteDetailsRepository;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<string, QuoteResponse>(_consumerConfig).SetValueDeserializer(new KafkaDeserializer<QuoteResponse>()).Build();
            consumer.Subscribe(_topic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await consumer.ConsumeWithTracing((result) =>
                    {
                        ProcessMessage(result, locatesReportDataService, quoteDetailsRepository);
                    }, stoppingToken);
                }
            }
            finally
            {
                consumer.Close();
            }
        });
    }

    private void ProcessMessage(ConsumeResult<string, QuoteResponse> consumeResult, ILocatesReportDataService locatesReportDataService, IQuoteDetailsRepository quoteDetailsRepository)
    {
        var message = consumeResult.Message.Value;

        locatesReportDataService.AddLocatorQuoteResponse(new LocatorQuoteResponse(message));

        if (QuoteResponseStatus.FinalizedStatuses.Contains(message.Status))
        {
            quoteDetailsRepository.AddResponseDetailsJson(message.Id, message.DetailsJson);
        }
    }
}