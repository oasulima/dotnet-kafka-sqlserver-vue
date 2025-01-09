namespace Shared.Quote
{
    public class LocatorQuoteResponse
    {
        public required string Id { get; set; }
        public required string AccountId { get; set; }
        public required string Symbol { get; set; }

        public required QuoteResponseStatusEnum Status { get; set; }
        public required DateTime Time { get; set; }
        public string? ErrorMessage { get; set; }
        public required int ReqQty { get; set; }
        public int? FillQty { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public required string Source { get; set; }
        public required IList<QuoteSourceInfo> Sources { get; set; }

        public static LocatorQuoteResponse From(QuoteResponse quoteResponse)
        {
            var sources = quoteResponse.Sources;

            var discountedPrice = 0M;
            if (quoteResponse.FillQty > 0)
            {
                discountedPrice =
                    sources.Select(x => x.DiscountedPrice * x.Qty).Sum()
                    / (decimal)quoteResponse.FillQty;
            }

            return new LocatorQuoteResponse
            {
                Id = quoteResponse.Id,
                AccountId = quoteResponse.AccountId,
                Symbol = quoteResponse.Symbol,
                Status = quoteResponse.Status,
                Time = quoteResponse.Time,
                ErrorMessage = quoteResponse.ErrorMessage,
                ReqQty = quoteResponse.ReqQty,
                FillQty = quoteResponse.FillQty,
                Price = quoteResponse.Price,
                DiscountedPrice = discountedPrice,
                Source = string.Join(", ", sources.Select(x => x.Source).Distinct()),
                Sources = sources,
            };
        }
    }
}
