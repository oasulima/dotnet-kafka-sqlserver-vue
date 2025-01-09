using Reporting.API.Data.Models.DbModels;
using Reporting.API.Data.Models.DbParams;
using Shared;
using Shared.Quote;
using Shared.Reporting;
using Shared.Reporting.Requests;

namespace Reporting.API.Utility;

public static class Mappers
{
    public static GetLocatesReportDataDbParams ToLocatesReportDataDbParams(
        this LocatesReportDataRequest request
    )
    {
        return new GetLocatesReportDataDbParams
        {
            Skip = ThrowIfNull(request.Skip),
            Take = ThrowIfNull(request.Take),
            OrderBy = ThrowIfNull(request.OrderBy),
            From = request.From,
            To = request.To,
            Symbol = request.Symbol,
            Status = (byte?)request.Status,
            AccountId = request.AccountId,
            ProviderId = request.ProviderId,
        };
    }

    public static LocatesReportData ToLocatesReportData(this LocatesReportDataDb dataDb)
    {
        return new LocatesReportData
        {
            Id = dataDb.Id,
            AccountId = dataDb.AccountId,
            Symbol = dataDb.Symbol,
            Status = (QuoteResponseStatusEnum)dataDb.Status,
            Time = dataDb.Time,
            ReqQty = dataDb.ReqQty,
            FillQty = dataDb.FillQty,
            Price = dataDb.Price,
            DiscountedPrice = dataDb.DiscountedPrice,
            Fee = dataDb.Fee,
            DiscountedFee = dataDb.DiscountedFee,
            Profit = dataDb.Profit,
            Source = dataDb.Source,
            Sources = dataDb.Sources.ToQuoteSourceInfos(),
            ErrorMessage = dataDb.ErrorMessage,
            TotalCount = dataDb.TotalCount,
        };
    }

    public static LocatorQuoteResponseDb ToLocatorQuoteResponseDb(
        this LocatorQuoteResponse quoteResponse
    )
    {
        return new LocatorQuoteResponseDb
        {
            Id = quoteResponse.Id,
            AccountId = quoteResponse.AccountId,
            Symbol = quoteResponse.Symbol,
            Status = (byte)quoteResponse.Status,
            Time = quoteResponse.Time,
            ErrorMessage = quoteResponse.ErrorMessage,
            ReqQty = quoteResponse.ReqQty,
            FillQty = quoteResponse.FillQty ?? 0,
            Price = quoteResponse.Price ?? 0,
            DiscountedPrice = quoteResponse.DiscountedPrice ?? 0,
            Source = quoteResponse.Source,
            Sources = quoteResponse.Sources.ToQuoteSourceInfosDb(),
        };
    }

    public static QuoteSourceInfoDb ToQuoteSourceInfoDb(this QuoteSourceInfo sourceInfo)
    {
        return new QuoteSourceInfoDb
        {
            Provider = sourceInfo.Provider,
            Source = sourceInfo.Source,
            Price = sourceInfo.Price,
            Qty = sourceInfo.Qty,
            DiscountedPrice = sourceInfo.DiscountedPrice,
        };
    }

    public static LocatorQuoteResponse ToLocatorQuoteResponse(
        this LocatorQuoteResponseDb quoteResponseDb
    )
    {
        return new LocatorQuoteResponse
        {
            Id = quoteResponseDb.Id,
            AccountId = quoteResponseDb.AccountId,
            Symbol = quoteResponseDb.Symbol,
            Status = (QuoteResponseStatusEnum)quoteResponseDb.Status,
            Time = quoteResponseDb.Time,
            ErrorMessage = quoteResponseDb.ErrorMessage,
            ReqQty = quoteResponseDb.ReqQty,
            FillQty = quoteResponseDb.FillQty,
            Price = quoteResponseDb.Price,
            DiscountedPrice = quoteResponseDb.DiscountedPrice,
            Source = quoteResponseDb.Source,
            Sources = quoteResponseDb.Sources.ToQuoteSourceInfos(),
        };
    }

    public static QuoteSourceInfo ToQuoteSourceInfo(this QuoteSourceInfoDb sourceInfoDb)
    {
        return new QuoteSourceInfo
        {
            Provider = sourceInfoDb.Provider,
            Source = sourceInfoDb.Source,
            Price = sourceInfoDb.Price,
            Qty = sourceInfoDb.Qty,
            DiscountedPrice = sourceInfoDb.DiscountedPrice,
        };
    }

    public static QuoteSourceInfoDb[] ToQuoteSourceInfosDb(
        this IEnumerable<QuoteSourceInfo> sourceInfos
    )
    {
        return sourceInfos.Select(ToQuoteSourceInfoDb).ToArray();
    }

    public static QuoteSourceInfo[] ToQuoteSourceInfos(
        this IEnumerable<QuoteSourceInfoDb> sourceInfosDb
    )
    {
        return sourceInfosDb.Select(ToQuoteSourceInfo).ToArray();
    }

    public static LocatesReportData[] ToLocatesReportData(
        this IEnumerable<LocatesReportDataDb> items
    )
    {
        return items.Select(x => x.ToLocatesReportData()).ToArray();
    }

    public static InternalInventoryItemDb ToInternalInventoryItemDb(this InternalInventoryItem item)
    {
        return new InternalInventoryItemDb
        {
            Id = item.Id,
            Price = item.Price,
            Quantity = item.Quantity,
            Source = item.Source,
            Status = item.Status,
            Symbol = item.Symbol,
            Tag = item.Tag,
            CoveredInvItemId = item.CoveredInvItemId,
            Version = item.Version,
            CreatingType = item.CreatingType,
            CreatedAt = item.CreatedAt,
            SoldQuantity = item.SoldQuantity,
        };
    }

    public static InternalInventoryItem ToInternalInventoryItem(this InternalInventoryItemDb dbItem)
    {
        return new InternalInventoryItem
        {
            Id = dbItem.Id,
            Price = dbItem.Price,
            Quantity = dbItem.Quantity,
            Source = dbItem.Source,
            Status = dbItem.Status,
            Symbol = dbItem.Symbol,
            Tag = dbItem.Tag,
            CoveredInvItemId = dbItem.CoveredInvItemId,
            Version = dbItem.Version,
            CreatingType = dbItem.CreatingType,
            CreatedAt = dbItem.CreatedAt,
            SoldQuantity = dbItem.SoldQuantity,
        };
    }

    public static QuoteRequestDb ToQuoteRequestDb(this QuoteRequest quoteRequest)
    {
        return new QuoteRequestDb
        {
            Id = quoteRequest.Id,
            AccountId = quoteRequest.AccountId,
            RequestType = quoteRequest.RequestType,
            Symbol = quoteRequest.Symbol,
            Quantity = quoteRequest.Quantity,
            AllowPartial = quoteRequest.AllowPartial,
            AutoApprove = quoteRequest.AutoApprove,
            MaxPriceForAutoApprove = quoteRequest.MaxPriceForAutoApprove,
            Time = quoteRequest.Time,
        };
    }

    private static T ThrowIfNull<T>(T? value)
        where T : class
    {
        if (value == null)
        {
            throw new Exception("Value cannot be null");
        }

        return value;
    }

    private static T ThrowIfNull<T>(T? value)
        where T : struct
    {
        if (value == null)
        {
            throw new Exception("Value cannot be null");
        }

        return value.Value;
    }
}
