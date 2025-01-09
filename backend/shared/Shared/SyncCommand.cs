using Newtonsoft.Json.Serialization;

namespace Shared;

public abstract record SyncCommand
{
    public enum CacheTypeEnum
    {
        ProviderSettings,
    }

    public record InvalidateCache(CacheTypeEnum CacheType) : SyncCommand;

    public record EnableProvider(string ProviderId) : SyncCommand;
}

public class SyncCommandBinder : ISerializationBinder
{
    public readonly Type[] KnownTypes =
    [
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
