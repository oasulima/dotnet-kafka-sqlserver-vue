using Microsoft.Extensions.Options;
using Admin.API.Options;
using Admin.API.Services.Interfaces;
using Shared;

namespace Admin.API.Services;

public class KafkaEventSender : IEventSender
{
    private readonly KafkaOptions _kafkaOptions;

    public KafkaEventSender(IOptions<KafkaOptions> kafkaOptions)
    {
        _kafkaOptions = kafkaOptions.Value;
    }

    public void SendLocatorQuoteRequest(QuoteRequest quoteRequest)
    {
        KafkaUtils.Produce(_kafkaOptions.Servers, _kafkaOptions.LocatorQuoteRequestTopic, quoteRequest);
    }

    public void SendEnableProviderCommand(string providerId)
    {
        var messageValue = new SyncCommand.EnableProvider(providerId);
        KafkaUtils.Produce(_kafkaOptions.Servers, _kafkaOptions.InvalidateCacheCommandTopic, messageValue);
    }
}