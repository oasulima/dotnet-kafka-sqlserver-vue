namespace Locator.API.Models.Options;

public class AppOptions
{
    public required TimeOnly DayDataCleanupTimeUtc { get; set; }
    public required TimeSpan NotificationSenderInterval { get; set; }
}
