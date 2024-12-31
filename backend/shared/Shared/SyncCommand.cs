using Newtonsoft.Json.Serialization;

namespace Shared;

public abstract record SyncCommand
{
    public enum CacheTypeEnum
    {
        ProviderSettings
    }
    public record InvalidateCache(CacheTypeEnum CacheType) : SyncCommand;

    public record EnableProvider(string ProviderId) : SyncCommand;

    // public enum CommandTypeEnum
    // {
    //     InvalidateCache,
    //     EnableProvider
    // }

    // public abstract record CommandType()
    // {

    //     public record KeyPress(char key) : WebEvent;
    //     public record Paste(string text) : WebEvent;
    //     public record Click(int x, int y) : WebEvent;
    // }

    // public string? ProviderId { get; set; }

    // public CacheTypeEnum? CacheType { get; set; }

    // public required CommandTypeEnum Command { get; set; }

    // public override string ToString()
    // {
    //     return $"{nameof(Command)}: [{Command}], " +
    //     $"{nameof(CacheType)}: [{CacheType}], " +
    //     $"{nameof(ProviderId)}: [{ProviderId}]";
    // }
}

public class SyncCommandBinder : ISerializationBinder
{
    public readonly Type[] KnownTypes = [
        typeof(SyncCommand.EnableProvider),
        typeof(SyncCommand.InvalidateCache),
    ];

    public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
    {
        assemblyName = null;
        typeName = serializedType.Name;
    }

    public Type BindToType(string? assemblyName, string typeName)
    {
        return KnownTypes.Single(t => t.Name == typeName);
    }
}