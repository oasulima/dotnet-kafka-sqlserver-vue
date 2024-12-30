﻿using Confluent.Kafka;
using Newtonsoft.Json;

namespace TradeZero.Locator.Emulator.Utility;

public static class KafkaListenerUtils
{
    public static T GetMessage<T>(this ConsumeResult<string, string> consumeResult)
    {
        var rawMessage = consumeResult.Message.Value;

        if (string.IsNullOrWhiteSpace(rawMessage))
        {
            throw new Exception($"An empty message was receive from {consumeResult.Topic} kafka topic");
        }

        var result = JsonConvert.DeserializeObject<T>(rawMessage);
        if (result == null)
        {
            throw new Exception($"Message from {consumeResult.Topic} kafka topic couldn't be deserialized");
        }

        return result;
    }
}