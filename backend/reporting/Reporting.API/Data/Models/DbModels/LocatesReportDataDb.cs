namespace Reporting.API.Data.Models.DbModels;

public record LocatesReportDataDb(
    string Id,
    string AccountId,
    string Symbol,
    byte Status,
    DateTime Time,
    int ReqQty,
    int? FillQty,
    decimal? Price,
    decimal? DiscountedPrice,
    decimal? Fee,
    decimal? DiscountedFee,
    decimal? Profit,
    string Source,
    QuoteSourceInfoDb[] Sources,
    string? ErrorMessage,
    int TotalCount
);