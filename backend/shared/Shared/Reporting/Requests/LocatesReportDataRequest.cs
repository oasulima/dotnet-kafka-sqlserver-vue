using System;

namespace Shared.Reporting.Requests
{
    public class LocatesReportDataRequest
    {
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public string OrderBy { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public QuoteResponseStatusEnum? Status { get; set; }
        public string? Symbol { get; set; }
        public string? AccountId { get; set; }
        public string? ProviderId { get; set; }
    }
}
