using System.Runtime.CompilerServices;
using Confluent.Kafka;
using Confluent.Kafka.Extensions.Diagnostics;
using Newtonsoft.Json;

namespace Shared;

public static class KafkaUtils
{
    public static Task ConsumeWithTracing<TKey, TValue>(
        this IConsumer<TKey, TValue> consumer,
        Action<ConsumeResult<TKey, TValue>> ProcessMessage,
        CancellationToken cancellationToken,
        [CallerFilePath] string sourceFilePath = ""
    )
    {
        return consumer.ConsumeWithInstrumentation(
            (result, token) =>
            {
                using var activity = TracingConfiguration.StartActivity(
                    $"{sourceFilePath} process message"
                );
                try
                {
                    if (result == null)
                    {
                        throw new Exception("consume: result is null");
                    }
                    ProcessMessage(result);
                }
                catch (Exception e)
                {
                    activity?.LogException(e);
                }
                return Task.CompletedTask;
            },
            cancellationToken
        );
    }

    public static void Produce<TValue>(
        string kafkaServers,
        string topic,
        TValue messageValue,
        Action<DeliveryReport<string, TValue>>? deliveryHandler = null
    )
        where TValue : class
    {
        var _producerConfig = new ProducerConfig { BootstrapServers = kafkaServers };
        using var producer = new ProducerBuilder<string, TValue>(_producerConfig)
            .SetValueSerializer(new KafkaSerializer<TValue>())
            .BuildWithInstrumentation();

        var message = new Message<string, TValue> { Value = messageValue };

        producer.Produce(topic, message, deliveryHandler);
        producer.Flush();
    }

    public class KafkaSerializer<TValue> : ISerializer<TValue>
    {
        public byte[] Serialize(TValue data, SerializationContext context)
        {
            var serializerSettings = data switch
            {
                SyncCommand => new JsonSerializerSettings(Converter.Settings)
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    SerializationBinder = new SyncCommandBinder(),
                },
                _ => new JsonSerializerSettings(Converter.Settings),
            };
            var data_str = Converter.Serialize(data, serializerSettings);
            return Serializers.Utf8.Serialize(data_str, context);
        }
    }
}

public class KafkaDeserializer<TValue> : IDeserializer<TValue>
{
    public TValue Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var data_str = Deserializers.Utf8.Deserialize(data, false, context);
        var serializerSettings = new JsonSerializerSettings(Converter.Settings);

        if (typeof(TValue).Name == typeof(SyncCommand).Name)
        {
            serializerSettings.TypeNameHandling = TypeNameHandling.Objects;
            serializerSettings.SerializationBinder = new SyncCommandBinder();
        }
        var result = Converter.Deserialize<TValue>(data_str, serializerSettings);

        return result
            ?? throw new NullReferenceException(
                $"can't desesialize to {typeof(TValue)}: {data_str}"
            );
        ;
    }
}
