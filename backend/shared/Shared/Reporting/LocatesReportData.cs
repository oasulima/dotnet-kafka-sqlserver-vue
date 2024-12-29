using System;
using System.Collections.Generic;

namespace Shared.Reporting
{
    public class LocatesReportData
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Symbol { get; set; }
        public QuoteResponseStatusEnum Status { get; set; }
        public DateTime Time { get; set; }
        public int ReqQty { get; set; }
        public int? FillQty { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? Fee { get; set; }
        public decimal? DiscountedFee { get; set; }
        public decimal? Profit { get; set; }
        public string Provider { get; set; }
        public string Source { get; set; }
        public IList<QuoteSourceInfo> Sources { get; set; }
        public string ErrorMessage { get; set; }
        public int TotalCount { get; set; }
    }
}
