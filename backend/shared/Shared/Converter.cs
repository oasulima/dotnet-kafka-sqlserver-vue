using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Shared;

public static class Converter
{
    public static string Serialize<T>(T obj, JsonSerializerSettings settings)
    {
        return JsonConvert.SerializeObject(obj, settings);
    }

    public static string Serialize<T>(T obj)
    {
        return Serialize(obj, Settings);
    }

    public static T? Deserialize<T>(string? json, JsonSerializerSettings serializerSettings)
    {
        return string.IsNullOrEmpty(json)
            ? default
            : JsonConvert.DeserializeObject<T>(json, serializerSettings);
    }

    public static object? Deserialize(string? json, JsonSerializerSettings serializerSettings)
    {
        return string.IsNullOrEmpty(json)
            ? default
            : JsonConvert.DeserializeObject(json, serializerSettings);
    }

    public static T? Deserialize<T>(string? json)
    {
        return Deserialize<T>(json, Settings);
    }

    public static object? Deserialize(string? json)
    {
        return Deserialize(json, Settings);
    }

    public static JsonSerializerSettings Settings { get; } =
        new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            Converters =
            [
                new StringEnumConverter(),
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AdjustToUniversal },
            ],
        };
}
