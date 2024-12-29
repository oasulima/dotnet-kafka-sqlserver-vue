using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Locator.API.Models.Internal;
using Locator.API.Models.Options;
using Locator.API.Services.Interfaces;
using Shared;

namespace Locator.API.HostedServices;

public class LocatorQuoteResponseKafkaListener : BackgroundService
{
    private readonly IAutoDisableProvidersService _autoDisableProvidersService;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    public LocatorQuoteResponseKafkaListener(IOptions<KafkaOptions> kafkaOptions,
        IAutoDisableProvidersService autoDisableProvidersService)
    {
        _autoDisableProvidersService = autoDisableProvidersService;
        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = kafkaOptions.Value.Servers,
            GroupId = kafkaOptions.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest,
        };
        _topic = kafkaOptions.Value.QuoteResponseTopic;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<string, QuoteResponse>(_consumerConfig)
                                        .SetValueDeserializer(new KafkaDeserializer<QuoteResponse>()
                                        )
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
        using var activity = TracingConfiguration.StartActivity("LocatorQuoteResponseKafkaListener ProcessMessage");
        try
        {
            var quoteResp = consumeResult.Message.Value;

            var quoteDetails = JsonConvert.DeserializeObject<QuoteResponseDetails>(quoteResp.DetailsJson);

            if (quoteDetails?.Quote != null)
            {
                var quote = quoteDetails.Quote;

                if (quoteResp.Status is QuoteResponseStatusEnum.WaitingAcceptance
                    or QuoteResponseStatusEnum.AutoAccepted
                    or QuoteResponseStatusEnum.AutoRejected
                    or QuoteResponseStatusEnum.RejectedDuplicate
                    or QuoteResponseStatusEnum.RejectedBadRequest
                    or QuoteResponseStatusEnum.NoInventory)
                {
                    foreach (var p in quote.ProviderQuoteRequests.Keys)
                    {
                        var isAnswered = quote.ProviderQuoteResponses.ContainsKey(p);
                        _autoDisableProvidersService.RegisterProviderQuote(p, quote.Symbol, isAnswered);
                    }
                }

                if (quoteResp.Status is QuoteResponseStatusEnum.Cancelled
                    or QuoteResponseStatusEnum.Expired
                    or QuoteResponseStatusEnum.Failed
                    or QuoteResponseStatusEnum.RejectedBadRequest
                    or QuoteResponseStatusEnum.RejectedDuplicate
                    or QuoteResponseStatusEnum.Partial
                    or QuoteResponseStatusEnum.Filled
                    or QuoteResponseStatusEnum.NoInventory
                    or QuoteResponseStatusEnum.AutoRejected)
                {
                    foreach (var p in quote.ProviderBuyOrderRequests
                                 .Where(x => x.Value.request.ValidTill > DateTime.MinValue)
                                 .Select(x => x.Key))
                    {
                        var isAnswered = quote.ProviderBuyOrderResponses.ContainsKey(p);
                        _autoDisableProvidersService.RegisterProviderBuy(
                            quote.ProviderBuyOrderRequests[p].provider,
                            quote.Symbol, isAnswered);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            activity.LogException(ex);
        }
    }
}