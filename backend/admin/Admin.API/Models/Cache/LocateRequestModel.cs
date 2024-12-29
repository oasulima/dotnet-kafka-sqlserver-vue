using System;
using Shared;

namespace Admin.API.Models.Cache;

public class LocateRequestModel
{
    public string Id { get; init; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public int QtyReq { get; set; }
    public int QtyOffer { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
    public string Source { get; set; } = string.Empty;
    public QuoteSourceInfo[] SourceDetails { get; set; }
}