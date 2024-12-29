using System;
using Shared;

namespace Reporting.API.Data.Models.DbModels;

public class InternalInventoryItemDb
{
    public string Id { get; set; }
    public int Version { get; set; }
    public string Symbol { get; set; }
    public int Quantity { get; set; }
    public int SoldQuantity { get; set; }
    public decimal Price { get; set; }
    public string Source { get; set; }
    public CreatingType CreatingType { get; set; }
    public string? Tag { get; set; }
    public string? CoveredInvItemId { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}