namespace Shared;

public class SyncCommand
{
    public enum CacheTypeEnum
    {
        ProviderSettings
    }

    public enum CommandTypeEnum
    {
        InvalidateCache,
        EnableProvider
    }

    public string? ProviderId { get; set; }

    public CacheTypeEnum? CacheType { get; set; }

    public required CommandTypeEnum Command { get; set; }

    public override string ToString()
    {
        return $"{nameof(Command)}: [{Command}], " +
        $"{nameof(CacheType)}: [{CacheType}], " +
        $"{nameof(ProviderId)}: [{ProviderId}]";
    }
}