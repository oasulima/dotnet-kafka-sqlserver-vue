namespace Shared.Locator;

public class InventoryItem
{
    public required string Id { get; init; }
    public required int Version { get; set; }
    public required string AccountId { get; set; }
    public required string Symbol { get; set; }
    public required int LocatedQuantity { get; set; }
    public required int AvailableQuantity { get; set; }
}
