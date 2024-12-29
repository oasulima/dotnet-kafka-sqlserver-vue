using System;

namespace Reporting.API.Data.Models.DbParams;

public class GetLocatesReportDataDbParams
{
    public int Skip { get; set; }
    public int Take { get; set; }
    public string OrderBy { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? Symbol { get; set; }
    public byte? Status { get; set; }
    public string? AccountId { get; set; }
    public string? ProviderId { get; set; }
}
