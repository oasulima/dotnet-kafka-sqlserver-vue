using Confluent.Kafka;
using Locator.UserEmulator;
using Locator.UserEmulator.Options;
using Locator.UserEmulator.Services;
using Locator.UserEmulator.Utility;
using Shared;

namespace Locator.UserEmulator.Kafka;

public class KafkaListener
{
    private readonly KafkaOptions _options;
    private readonly QuoteRegistrar _quoteRegistrar;

    public KafkaListener(KafkaOptions options, QuoteRegistrar quoteRegistrar)
    {
        _options = options;
        _quoteRegistrar = quoteRegistrar;
    }

    public Task Start(CancellationToken cts)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.Servers,
            GroupId = _options.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest,
        };
        var tasks = new List<Task>();
        for (int i = 0; i < _options.NumberOfListeners; i++)
        {
            var index = i;
            var task = Task.Run(
                () =>
                {
                    SharedData.Log($"Start Listener {index}");
                    using var consumer = new ConsumerBuilder<string, string>(config).Build();

                    consumer.Subscribe(_options.LocatorResponseTopic);

                    while (!cts.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cts);
                            _quoteRegistrar.RegisterQuoteResponse(
                                consumeResult.GetMessage<QuoteResponse>()
                            );
                        }
                        catch (Exception)
                        {
                            //TODO: logs
                        }
                    }

                    consumer.Close();
                    SharedData.Log($"Stop Listener {index}");
                },
                cts
            );
            tasks.Add(task);
        }

        return Task.WhenAll(tasks);
    }
}
