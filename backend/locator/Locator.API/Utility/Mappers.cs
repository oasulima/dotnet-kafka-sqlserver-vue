using Locator.API.Models.Internal;
using Locator.API.Services.Interfaces;
using Shared;
using Shared.Settings;

namespace Locator.API.Utility;

public static class Mappers
{
    public static ProviderSetting ToProviderSetting(this ProviderSettingRequest providerSettingReq)
    {
        return new ProviderSetting
        {
            Name = ThrowIfNull(providerSettingReq.Name),
            ProviderId = ThrowIfNull(providerSettingReq.ProviderId),
            Discount = ThrowIfNull(providerSettingReq.Discount),
            Active = ThrowIfNull(providerSettingReq.Active),
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

    public static ProviderSetting ToResponse(this ProviderSetting providerSetting)
    {
        return new ProviderSetting
        {
            Active = providerSetting.Active,
            Discount = providerSetting.Discount,
            ProviderId = providerSetting.ProviderId,
            Name = providerSetting.Name,
            BuyRequestTopic = providerSetting.BuyRequestTopic,
            BuyResponseTopic = providerSetting.BuyResponseTopic,
            QuoteRequestTopic = providerSetting.QuoteRequestTopic,
            QuoteResponseTopic = providerSetting.QuoteResponseTopic,
        };
    }

    public static ProviderBuyRequest ToProviderBuyRequest(this PriceCalculationInfo info)
    {
        return new ProviderBuyRequest
        {
            Id = Guid.NewGuid().ToString(),
            QuoteId = info.QuoteId,
            Symbol = info.Symbol,
            AccountId = info.AccountId,
            RequestedAssets = new List<PriceInfo>(),
        };
    }

    public static string ToResponseDetailsJson(this Quote quote)
    {
        var obj = new QuoteResponseDetails(
            PriceCalculationResult: quote.PriceCalculationResult,
            Quote: quote
        );

        return Converter.Serialize(obj);
    }

    public static string ToResponseDetailsJson(
        this Quote quote,
        QuoteResponseStatusEnum status,
        string? message
    )
    {
        var obj = new QuoteResponseDetails(
            PriceCalculationResult: quote.PriceCalculationResult,
            Status: status,
            Message: message,
            Quote: quote
        );

        return Converter.Serialize(obj);
    }

    public static string ToResponseDetailsJson(
        this QuoteRequest quoteRequest,
        QuoteResponseStatusEnum status,
        string? message
    )
    {
        var obj = new QuoteResponseDetails(
            QuoteRequest: quoteRequest,
            Status: status,
            Message: message
        );

        return Converter.Serialize(obj);
    }

    public static QuoteResponse ToQuoteResponse(
        this QuoteRequest quoteRequest,
        QuoteResponseStatusEnum status,
        string? errorMessage
    )
    {
        return new QuoteResponse
        {
            Id = quoteRequest.Id,
            AccountId = quoteRequest.AccountId,
            Symbol = quoteRequest.Symbol,
            Status = status,
            ErrorMessage = errorMessage,
            ReqQty = quoteRequest.Quantity,
            FillQty = null,
            Price = null,
            Sources = Array.Empty<QuoteSourceInfo>(),
            DetailsJson = quoteRequest.ToResponseDetailsJson(status, errorMessage),
        };
    }

    public static QuoteResponse ToQuoteResponse(
        this Quote quote,
        QuoteResponseStatusEnum status,
        string? errorMessage
    )
    {
        return new QuoteResponse
        {
            Id = quote.Id,
            AccountId = quote.AccountId,
            Symbol = quote.Symbol,
            Status = status,
            ErrorMessage = errorMessage,
            ReqQty = quote.RequestedQuantity,
            FillQty = quote.QuantityToBuy,
            Price = quote.Price,
            Sources = Array.Empty<QuoteSourceInfo>(),
            DetailsJson = quote.ToResponseDetailsJson(status, errorMessage),
        };
    }
}
