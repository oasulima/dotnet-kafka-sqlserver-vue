namespace Locator.API.Models.Options;

public class KafkaOptions
{
    public string Servers { get; set; }
    public string GroupId { get; set; }
    public string QuoteRequestTopic { get; set; }
    public string QuoteResponseTopic { get; set; }
    public string NotificationTopic { get; set; }
    public string AddInternalInventoryItemTopic { get; set; }
    public string InvalidateCacheCommandTopic { get; set; }
}