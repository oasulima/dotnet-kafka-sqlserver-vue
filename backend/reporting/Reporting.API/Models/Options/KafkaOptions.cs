namespace Reporting.API.Models.Options;

public class KafkaOptions
{
    public string Servers { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string QuoteRequestTopic { get; set; } = string.Empty;
    public string QuoteResponseTopic { get; set; } = string.Empty;
    public string InternalInventoryItemReportingTopic { get; set; } = string.Empty;
}