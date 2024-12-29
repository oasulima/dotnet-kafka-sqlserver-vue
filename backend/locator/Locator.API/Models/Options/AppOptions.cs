namespace Locator.API.Models.Options;

public class AppOptions
{
    public TimeOnly DayDataCleanupTimeUtc { get; set; }
    public TimeSpan NotificationSenderInterval { get; set; }
}