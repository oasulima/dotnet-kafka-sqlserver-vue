using Locator.API.Services.Interfaces;
using Locator.API.Utility;
using Shared;

namespace Locator.API.Models.Internal;

public class Quote
{
    public record QuoteKey(string Id, string AccountId);

    public record AccountIdentifier(string AccountId);

    private static HashSet<QuoteStatus> ActiveStatuses =
    [
        QuoteStatus.New,
        QuoteStatus.WaitingProvidersQuotes,
        QuoteStatus.WaitingAcceptance,
        QuoteStatus.Accepted,
    ];

    public enum QuoteStatus
    {
        New,
        WaitingProvidersQuotes,
        WaitingAcceptance,
        Accepted,
        NoInventory,
        Cancelled,
        Filled,
    }

    public QuoteKey Key => new QuoteKey(Id, AccountId);
    public AccountIdentifier Account => new AccountIdentifier(AccountId);

    public required string Id { get; init; }
    public required string Symbol { get; init; }
    public required string AccountId { get; init; }
    public int RequestedQuantity { get; set; }
    public decimal Price { get; set; }

    public int QuantityToBuy { get; set; }

    public bool WaitingProvidersTimeout { get; set; }

    public int ActualQuantity =>
        ProviderBuyOrderResponses
            .Values.SelectMany(x => x.response.BoughtAssets)
            .Sum(x => x.Quantity);

    private QuoteStatus _status;

    public QuoteStatus Status
    {
        get => _status;
        set
        {
            LastStatusUpdate = DateTime.UtcNow;
            _status = value;
        }
    }

    public bool IsActive => ActiveStatuses.Contains(Status);

    public Dictionary<string, ProviderQuoteResponse> ProviderQuoteResponses { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, ProviderQuoteRequest> ProviderQuoteRequests { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<
        string,
        (string provider, ProviderBuyRequest request)
    > ProviderBuyOrderRequests { get; } = new();

    public Dictionary<
        string,
        (string provider, ProviderBuyResponse response)
    > ProviderBuyOrderResponses { get; } = new();

    public PriceCalculationResult? PriceCalculationResult { get; set; }
    public decimal MaxPriceForAutoApprove { get; set; }
    public bool AllowPartial { get; set; }
    public bool AutoApprove { get; set; }

    public DateTime LastStatusUpdate { get; set; }
    public DateTime CreatedAt { get; set; }

    public static Quote FromQuoteRequest(QuoteRequest quoteRequest)
    {
        return new Quote()
        {
            Id = quoteRequest.Id,
            Status = Quote.QuoteStatus.New,
            //TODO: maybe it makes sense to store the whole quoteRequestObject in Quote object
            RequestedQuantity = quoteRequest.Quantity,
            AccountId = quoteRequest.AccountId,
            Symbol = quoteRequest.Symbol,
            MaxPriceForAutoApprove = quoteRequest.MaxPriceForAutoApprove,
            AllowPartial = quoteRequest.AllowPartial,
            AutoApprove = quoteRequest.AutoApprove,
        };
    }

    public ProviderBuyRequest ToProviderBuyRequest()
    {
        return new ProviderBuyRequest()
        {
            Id = Guid.NewGuid().ToString(),
            QuoteId = this.Id,
            Symbol = this.Symbol,
            AccountId = this.AccountId,
            RequestedAssets = [],
        };
    }

    public ProviderQuoteRequest ToProviderQuoteRequest()
    {
        return new ProviderQuoteRequest()
        {
            Id = Guid.NewGuid().ToString(),
            Quantity = this.RequestedQuantity,
            Symbol = this.Symbol,
            AccountId = this.AccountId,
            QuoteId = this.Id,
        };
    }

    public QuoteResponse ToQuoteResponse(QuoteResponseStatusEnum status)
    {
        return new QuoteResponse
        {
            DetailsJson = this.ToResponseDetailsJson(),
            Status = status,
            Id = this.Id,
            Symbol = this.Symbol,
            AccountId = this.AccountId,
            ReqQty = this.RequestedQuantity,
            FillQty = this.QuantityToBuy,
            Price = this.Price,
            // todo: extract sources calculation logic
            Sources = this
                .ProviderBuyOrderRequests.Values.SelectMany(x =>
                    x.request.RequestedAssets.Select(priceInfo => (x.provider, priceInfo))
                )
                .Select(x =>
                {
                    var provider = x.provider;
                    var priceInfo = x.priceInfo;
                    //TODO: most likely can be optimized
                    var discountedPriceObject = this.PriceCalculationResult?.Offers.FirstOrDefault(
                        ddp => ddp.Source == priceInfo.Source && ddp.Price == priceInfo.Price
                    );
                    return new QuoteSourceInfo()
                    {
                        Provider = provider,
                        Source = priceInfo.Source!,
                        Price = priceInfo.Price,
                        Qty = priceInfo.Quantity,
                        DiscountedPrice = discountedPriceObject?.DiscountedPrice ?? 0,
                    };
                })
                .ToArray(),
        };
    }
}
