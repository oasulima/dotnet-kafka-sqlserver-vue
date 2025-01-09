namespace Reporting.API.Models.Options;

public class KafkaOptions
{
    public required string Servers { get; set; }
    public required string GroupId { get; set; }
    public required string QuoteRequestTopic { get; set; }
    public required string QuoteResponseTopic { get; set; }
    public required string InternalInventoryItemReportingTopic { get; set; }
}
