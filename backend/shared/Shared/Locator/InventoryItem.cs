namespace Shared.Locator;

public class InventoryItem
{
    public string Id { get; set; }
    public int Version { get; set; }
    public string AccountId { get; set; }
    public string Symbol { get; set; }
    public int LocatedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public string OriginalSource { get; set; }
    public decimal OriginalPrice { get; set; }
}