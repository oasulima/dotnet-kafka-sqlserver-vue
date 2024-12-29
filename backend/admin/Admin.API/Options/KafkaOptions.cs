namespace Admin.API.Options;

public class KafkaOptions
{
    public string Servers { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string LocatorQuoteResponseTopic { get; set; } = string.Empty;
    public string NotificationTopic { get; set; } = string.Empty;
    public string InvalidateCacheCommandTopic { get; set; } = string.Empty;
    public string InternalInventoryItemChangeTopic { get; set; } = string.Empty;
    public string LocatorQuoteRequestTopic { get; set; } = string.Empty;
}