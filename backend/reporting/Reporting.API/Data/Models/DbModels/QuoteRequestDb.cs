using System;
using Shared;

namespace Reporting.API.Data.Models.DbModels;

public class QuoteRequestDb
{
    public required string Id { get; set; }
    public required string AccountId { get; set; }
    public required QuoteRequest.RequestTypeEnum RequestType { get; set; }
    public string? Symbol { get; set; }
    public required int Quantity { get; set; }
    public required bool AllowPartial { get; set; }
    public required bool AutoApprove { get; set; }
    public required decimal MaxPriceForAutoApprove { get; set; }
    public required DateTime Time { get; set; }
}
