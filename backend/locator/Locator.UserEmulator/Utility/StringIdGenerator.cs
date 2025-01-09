namespace Locator.UserEmulator.Utility;

public static class StringIdGenerator
{
    private static readonly string Prefix = Environment.MachineName;

    public static string GenerateId()
    {
        return $"{Prefix}{Guid.NewGuid()}";
    }
}
