using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Locator.API.Models.Options;
using Locator.API.Services.Interfaces;
using Shared;

namespace Locator.API.Services;

public class KafkaEventSender : IEventSender
{
    private readonly KafkaOptions _kafkaOptions;
    private string QuoteResponseTopic => _kafkaOptions.QuoteResponseTopic!;
    private string AddInternalInventoryItemTopic => _kafkaOptions.AddInternalInventoryItemTopic!;
    private string InvalidateCacheCommandTopic => _kafkaOptions.InvalidateCacheCommandTopic!;

    public KafkaEventSender(IOptions<KafkaOptions> options)
    {
        _kafkaOptions = options.Value;
    }

    public void SendProviderQuoteRequest(string topic, ProviderQuoteRequest providerQuoteRequest)
    {
        KafkaUtils.Produce(_kafkaOptions.Servers, topic, providerQuoteRequest,
            dr => HandleDeliveryReport(dr, nameof(SendProviderQuoteRequest)));
    }

    public void SendQuoteResponse(QuoteResponse quoteResponse)
    {
        KafkaUtils.Produce(_kafkaOptions.Servers, QuoteResponseTopic, quoteResponse,
            dr => HandleDeliveryReport(dr, nameof(SendQuoteResponse)));
    }

    public void SendLocatorQuoteRequest(QuoteRequest quoteRequest)
    {
        KafkaUtils.Produce(_kafkaOptions.Servers, _kafkaOptions.QuoteRequestTopic, quoteRequest,
            dr => HandleDeliveryReport(dr, nameof(SendLocatorQuoteRequest)));
    }

    public void SendProviderBuyOrderRequest(string providerConfigurationBuyRequestTopic,
        ProviderBuyRequest orderRequest)
    {
        KafkaUtils.Produce(_kafkaOptions.Servers, providerConfigurationBuyRequestTopic, orderRequest,
            dr => HandleDeliveryReport(dr, nameof(SendProviderBuyOrderRequest)));
    }

    public void SendAddInternalInventoryItemRequest(AddInternalInventoryItemRequest request)
    {
        KafkaUtils.Produce(_kafkaOptions.Servers, AddInternalInventoryItemTopic, request,
            dr => HandleDeliveryReport(dr, nameof(SendAddInternalInventoryItemRequest)));
    }

    public void SendInvalidateCacheCommand(SyncCommand invalidateCacheCommand)
    {
        KafkaUtils.Produce(_kafkaOptions.Servers, InvalidateCacheCommandTopic, invalidateCacheCommand,
            dr => HandleDeliveryReport(dr, nameof(SendInvalidateCacheCommand)));
    }

    private void HandleDeliveryReport<TKey, TValue>(DeliveryReport<TKey, TValue> dr, string methodName)
    {
        switch (dr.Status)
        {
            case PersistenceStatus.NotPersisted:
                break;
            case PersistenceStatus.PossiblyPersisted:
                break;
        }
    }
}