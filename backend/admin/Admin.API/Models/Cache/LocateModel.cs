using System;
using Shared;

namespace Admin.API.Models.Cache;

public class LocateModel
{
    public required string QuoteId { get; set; }
    public required string AccountId { get; set; }
    public required DateTime Time { get; set; }
    public required string Symbol { get; set; }
    public required int ReqQty { get; set; }
    public required int QtyFill { get; set; }
    public required decimal DiscountedPrice { get; set; }
    public required decimal Price { get; set; }
    public required QuoteResponseStatusEnum Status { get; set; }
    public string? ErrorMessage { get; set; }
    public required string Source { get; set; }
    public required IList<QuoteSourceInfo> SourceDetails { get; set; }
}
