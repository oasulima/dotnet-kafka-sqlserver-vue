namespace Shared.Reporting
{
    public class LocatesReportData
    {
        public required string Id { get; set; }
        public required string AccountId { get; set; }
        public required string Symbol { get; set; }
        public required QuoteResponseStatusEnum Status { get; set; }
        public required DateTime Time { get; set; }
        public required int ReqQty { get; set; }
        public int? FillQty { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? Fee { get; set; }
        public decimal? DiscountedFee { get; set; }
        public decimal? Profit { get; set; }
        public required string Source { get; set; }
        public required IList<QuoteSourceInfo> Sources { get; set; }
        public string? ErrorMessage { get; set; }
        public required int TotalCount { get; set; }
    }
}
