using Confluent.Kafka;
using Locator.API.Models.Options;
using Locator.API.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared;

namespace Locator.API.Services;

public class NotificationEventSender : INotificationEventSender
{
    private readonly KafkaOptions _kafkaOptions;

    private string NotificationTopic => _kafkaOptions.NotificationTopic!;

    public NotificationEventSender(IOptions<KafkaOptions> options)
    {
        _kafkaOptions = options.Value;
    }

    public void Send(GroupedNotification[] notifications)
    {
        try
        {
            KafkaUtils.Produce(
                _kafkaOptions.Servers,
                NotificationTopic,
                notifications,
                ProcessDeliveryReport
            );
        }
        catch (Exception)
        {
            // NotificationEventSender shouldn't throw exceptions, just log.
        }
    }

    private void ProcessDeliveryReport<TKey, TValue>(DeliveryReport<TKey, TValue> deliveryReport)
    {
        switch (deliveryReport.Status)
        {
            case PersistenceStatus.NotPersisted:
                break;

            case PersistenceStatus.PossiblyPersisted:
                break;
        }
    }
}
