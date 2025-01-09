using Confluent.Kafka;
using Locator.UserEmulator;
using Locator.UserEmulator.Options;
using Newtonsoft.Json;
using Shared;

namespace Locator.UserEmulator.Kafka;

public interface IKafkaSender
{
    void SendQuote(QuoteRequest quote);
}

public class KafkaSender : IDisposable, IKafkaSender
{
    private readonly KafkaOptions _options;
    private readonly IProducer<Null, string> _producer;

    public KafkaSender(KafkaOptions options)
    {
        _options = options;

        var config = new ProducerConfig { BootstrapServers = _options.Servers };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public void SendQuote(QuoteRequest quote)
    {
        _producer.Produce(
            _options.LocatorRequestTopic,
            new Message<Null, string>() { Value = Converter.Serialize(quote) },
            dr =>
            {
                switch (dr.Status)
                {
                    case PersistenceStatus.NotPersisted:
                        SharedData.Log(
                            $"quote: {quote.Id}; RequestType:{quote.RequestType}; Cannot send a message to kafka. Reason {dr.Error.Reason}. Code {dr.Error.Code}"
                        );
                        break;
                    case PersistenceStatus.PossiblyPersisted:
                        SharedData.Log(
                            $"quote: {quote.Id}; RequestType:{quote.RequestType}; Message haven't been acknowledged by kafka. Reason {dr.Error.Reason ?? "unknown"}. Code {dr.Error.Code} "
                        );
                        break;
                }
            }
        );
    }

    public void Dispose()
    {
        try
        {
            _producer.Dispose();
        }
        catch
        {
            //noop
        }
    }
}
