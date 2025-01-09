using Shared;

namespace Admin.API.Models.Cache;

public class LocateRequestModel
{
    public required string Id { get; init; }
    public required string AccountId { get; set; }
    public required DateTime Time { get; set; }
    public required string Symbol { get; set; }
    public required int QtyReq { get; set; }
    public required int QtyOffer { get; set; }
    public required decimal Price { get; set; }
    public required decimal DiscountedPrice { get; set; }
    public required string Source { get; set; }
    public required IList<QuoteSourceInfo> SourceDetails { get; set; }
}
