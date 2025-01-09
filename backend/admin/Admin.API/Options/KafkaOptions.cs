namespace Admin.API.Options;

public class KafkaOptions
{
    public required string Servers { get; set; }
    public required string GroupId { get; set; }
    public required string LocatorQuoteResponseTopic { get; set; }
    public required string NotificationTopic { get; set; }
    public required string InvalidateCacheCommandTopic { get; set; }
    public required string InternalInventoryItemChangeTopic { get; set; }
    public required string LocatorQuoteRequestTopic { get; set; }
}
