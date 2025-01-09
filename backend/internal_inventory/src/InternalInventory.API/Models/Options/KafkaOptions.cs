namespace InternalInventory.API.Models.Options;

public class KafkaOptions
{
    public required string Servers { get; set; }
    public required string QuoteRequestTopic { get; set; }
    public required string BuyRequestTopic { get; set; }
    public required string OrderReportTopic { get; set; }
    public required string QuoteResponseTopic { get; set; }
    public required string GroupId { get; set; }
    public required string AddInventoryItemTopic { get; set; }
    public required string InternalInventoryItemReportingTopic { get; set; }
}
