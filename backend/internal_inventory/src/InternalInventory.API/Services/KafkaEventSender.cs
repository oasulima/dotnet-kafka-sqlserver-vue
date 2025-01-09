using Confluent.Kafka;
using InternalInventory.API.Models.Options;
using InternalInventory.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using Shared;

namespace InternalInventory.API.Services
{
    public class KafkaEventSender : IEventSender
    {
        private readonly IOptions<KafkaOptions> _options;
        private const int DefaultRetryCounts = 3;
        private string OrderReportTopic => _options.Value.OrderReportTopic;
        private string QuoteResponseTopic => _options.Value.QuoteResponseTopic;
        private string InternalInventoryItemReportingTopic =>
            _options.Value.InternalInventoryItemReportingTopic;

        private readonly string _kafkaServers;

        public KafkaEventSender(IOptions<KafkaOptions> options)
        {
            _kafkaServers = options.Value.Servers;

            _options = options;
        }

        private void SendMessageWithRetries<TValue>(
            string topic,
            TValue messageValue,
            int retryCount = DefaultRetryCounts
        )
            where TValue : class
        {
            if (retryCount <= 0)
                return;

            KafkaUtils.Produce(
                _kafkaServers,
                topic,
                messageValue,
                dr =>
                {
                    if (dr.Error.IsError) { }

                    if (dr.Status != PersistenceStatus.Persisted)
                    {
                        var logMessage = $"message wasn't persisted by kafka topic {topic}. ";

                        if (retryCount > 1)
                        {
                            SendMessageWithRetries(topic, messageValue, retryCount - 1);
                        }
                        else { }
                    }
                }
            );
        }

        public void SendBuyResponseEvent(ProviderBuyResponse orderEvent)
        {
            SendMessageWithRetries(OrderReportTopic, orderEvent);
        }

        public void SendQuoteResponseReadyEvent(ProviderQuoteResponse response)
        {
            SendMessageWithRetries(QuoteResponseTopic, response);
        }

        public void SendInternalInventoryItemChangeEvent(InternalInventoryItem item)
        {
            SendMessageWithRetries(InternalInventoryItemReportingTopic, item);
        }
    }
}
