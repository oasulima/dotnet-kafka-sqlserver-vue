using System;
using Shared;

namespace Reporting.API.Data.Models.DbModels;

public class QuoteRequestDb
{
    public string Id { get; set; }
    public string AccountId { get; set; }
    public QuoteRequest.RequestTypeEnum RequestType { get; set; }
    public string? Symbol { get; set; }
    public int Quantity { get; set; }
    public bool AllowPartial { get; set; }
    public bool AutoApprove { get; set; }
    public decimal MaxPriceForAutoApprove { get; set; }
    public DateTime Time { get; set; }
}
