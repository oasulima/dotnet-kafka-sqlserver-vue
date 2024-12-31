using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
        [CallerFilePath] string sourceFilePath = "")
    {
        return consumer.ConsumeWithInstrumentation((result, token) =>
        {
            using var activity = TracingConfiguration.StartActivity($"{sourceFilePath} process message");
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
        }, cancellationToken);
    }

    public static void Produce<TValue>(string kafkaServers, string topic, TValue messageValue,
        Action<DeliveryReport<string, TValue>> deliveryHandler = null) where TValue : class
    {
        var _producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaServers
        };
        using var producer =
           new ProducerBuilder<string, TValue>(_producerConfig)
               .SetValueSerializer(new KafkaSerializer<TValue>())
           .BuildWithInstrumentation();

        var message = new Message<string, TValue>
        {
            Value = messageValue
        };

        producer.Produce(topic, message, deliveryHandler);
        producer.Flush();
    }

    public class KafkaSerializer<TValue> : ISerializer<TValue>
    {
        public byte[] Serialize(TValue data, SerializationContext context)
        {
            var serializerSettings = data switch
            {
                SyncCommand => new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    SerializationBinder = new SyncCommandBinder()
                },
                _ => new JsonSerializerSettings()
            };
            serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            var data_str = JsonConvert.SerializeObject(data, serializerSettings);
            return Serializers.Utf8.Serialize(data_str, context);
        }
    }
}

public class KafkaDeserializer<TValue> : IDeserializer<TValue>
{
    public TValue Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var data_str = Deserializers.Utf8.Deserialize(data, false, context); 
        var serializerSettings = new JsonSerializerSettings();

        if (typeof(TValue).Name == typeof(SyncCommand).Name)
        {
            serializerSettings.TypeNameHandling = TypeNameHandling.Objects;
            serializerSettings.SerializationBinder = new SyncCommandBinder();
        }
        serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        return JsonConvert.DeserializeObject<TValue>(data_str, serializerSettings);
    }
}