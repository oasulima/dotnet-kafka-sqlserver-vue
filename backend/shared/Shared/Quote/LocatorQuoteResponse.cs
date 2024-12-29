using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Quote
{
    public class LocatorQuoteResponse
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Symbol { get; set; }

        public QuoteResponseStatusEnum Status { get; set; }
        public DateTime Time { get; set; }
        public string ErrorMessage { get; set; }
        public int ReqQty { get; set; }
        public int? FillQty { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public string Source { get; set; }
        public IList<QuoteSourceInfo> Sources { get; set; }

        public LocatorQuoteResponse()
        {
        }

        public LocatorQuoteResponse(QuoteResponse quoteResponse)
        {
            var sources = quoteResponse.Sources ?? Array.Empty<QuoteSourceInfo>();

            var discountedPrice = 0M;
            if (quoteResponse.FillQty > 0)
            {
                discountedPrice = sources.Select(x => x.DiscountedPrice * x.Qty).Sum() / (decimal)quoteResponse.FillQty;
            }

            Id = quoteResponse.Id;
            AccountId = quoteResponse.AccountId;
            Symbol = quoteResponse.Symbol;
            Status = quoteResponse.Status;
            Time = quoteResponse.Time;
            ErrorMessage = quoteResponse.ErrorMessage;
            ReqQty = quoteResponse.ReqQty;
            FillQty = quoteResponse.FillQty;
            Price = quoteResponse.Price;
            DiscountedPrice = discountedPrice;
            Source = string.Join(", ", sources.Select(x => x.Source).Distinct());
            Sources = sources;
        }
    }
}
