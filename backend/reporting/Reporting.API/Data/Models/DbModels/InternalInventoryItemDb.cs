using Shared;
using static Shared.InternalInventoryItem;

namespace Reporting.API.Data.Models.DbModels;

public class InternalInventoryItemDb
{
    public required string Id { get; set; }
    public required int Version { get; set; }
    public required string Symbol { get; set; }
    public required int Quantity { get; set; }
    public required int SoldQuantity { get; set; }
    public required decimal Price { get; set; }
    public required string Source { get; set; }
    public required CreatingType CreatingType { get; set; }
    public string? Tag { get; set; }
    public string? CoveredInvItemId { get; set; }
    public required State Status { get; set; }
    public required DateTime CreatedAt { get; set; }
}
