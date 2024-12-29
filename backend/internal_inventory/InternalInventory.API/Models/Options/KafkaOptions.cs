namespace InternalInventory.API.Models.Options;

public class KafkaOptions
{
    public string Servers { get; set; } = null!; //Ignore nullable after constructor, it will always be initialized in startup
    public string QuoteRequestTopic { get; set; } = null!;
    public string BuyRequestTopic { get; set; } = null!;
    public string OrderReportTopic { get; set; } = null!;
    public string QuoteResponseTopic { get; set; } = null!;
    public string GroupId { get; set; } = null!;
    public string AddInventoryItemTopic { get; set; } = null!;
    public string InternalInventoryItemReportingTopic { get; set; } = null!;
}