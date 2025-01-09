using Locator.API.Models.Internal;
using Locator.API.Models.Options;
using Locator.API.Services.Interfaces;
using Locator.API.Storages.Interfaces;
using Locator.API.Utility;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Locator;
using Shared.Quote;
using static Locator.API.Services.PriceCalculationService;
using PriceInfo = Shared.PriceInfo;

namespace Locator.API.Services;

public class LocatorService : ILocatorService
{
    private readonly IQuoteStorage _quoteStorage;
    private readonly IEventSender _eventSender;
    private readonly IInventoryService _inventoryService;
    private readonly ISettingService _settingService;
    private readonly IPriceCalculationService _priceCalculationService;
    private readonly IProviderSettingStorage _providerSettingStorage;
    private readonly IOptions<QuoteTimeoutOptions> _timeoutOptions;
    private readonly INotificationService _notificationService;
    private readonly IAutoDisableProvidersService _autoDisableProvidersService;

    public LocatorService(
        IQuoteStorage quoteStorage,
        IEventSender eventSender,
        IInventoryService inventoryService,
        ISettingService settingService,
        IPriceCalculationService priceCalculationService,
        IProviderSettingStorage providerSettingStorage,
        IOptions<QuoteTimeoutOptions> timeoutOptions,
        INotificationService notificationService,
        IAutoDisableProvidersService autoDisableProvidersService
    )
    {
        _quoteStorage = quoteStorage;
        _eventSender = eventSender;
        _inventoryService = inventoryService;
        _settingService = settingService;
        _priceCalculationService = priceCalculationService;
        _providerSettingStorage = providerSettingStorage;
        _timeoutOptions = timeoutOptions;
        _notificationService = notificationService;
        _autoDisableProvidersService = autoDisableProvidersService;
    }

    #region ProcessQuoteRequest

    public void CreateQuote(QuoteRequest quoteRequest, DateTime timeRequestArrived)
    {
        var quoteKey = new Quote.QuoteKey(quoteRequest.Id, quoteRequest.AccountId);

        try
        {
            var symbol = quoteRequest.Symbol;

            InternalProcessQuoteRequest(quoteRequest, timeRequestArrived);
        }
        finally
        {
            _quoteStorage.ReleaseQuote(quoteKey);
        }
    }

    private void InternalProcessQuoteRequest(QuoteRequest quoteRequest, DateTime timeRequestArrived)
    {
        var quote = Quote.FromQuoteRequest(quoteRequest);
        quote.CreatedAt = timeRequestArrived;

        if (!_quoteStorage.TryAddQuoteWithLock(quote))
        {
            SendDuplicateQuoteResponse(
                quoteRequest,
                $"quote request id is not unique {quoteRequest.Id}"
            );
            return;
        }

        var providers = _settingService.GetActiveExternalProviderSettings();

        foreach (var provider in providers)
        {
            if (string.IsNullOrEmpty(provider.QuoteRequestTopic))
            {
                continue;
            }

            if (_autoDisableProvidersService.IsProviderDisabled(provider.ProviderId, quote.Symbol))
            {
                continue;
            }

            var request = quote.ToProviderQuoteRequest();

            request.ValidTill = DateTime.UtcNow.Add(_timeoutOptions.Value.MaxProviderQuoteWait / 2);

            quote.ProviderQuoteRequests[provider.ProviderId] = request;
            _eventSender.SendProviderQuoteRequest(provider.QuoteRequestTopic, request);
        }

        quote.Status = Quote.QuoteStatus.WaitingProvidersQuotes;
        SendQuoteResponse(quote, QuoteResponseStatusEnum.RequestAccepted, null);
    }

    private void SendDuplicateQuoteResponse(QuoteRequest quoteRequest, string message)
    {
        SendQuoteResponse(quoteRequest, QuoteResponseStatusEnum.RejectedDuplicate, message);
    }

    #endregion

    #region Process Quote Before it is approved

    public void ProcessProviderQuoteResponse(
        ProviderQuoteResponse providerQuoteResponse,
        string providerId
    )
    {
        var key = new Quote.QuoteKey(
            providerQuoteResponse.QuoteId,
            providerQuoteResponse.AccountId
        );
        try
        {
            if (providerQuoteResponse.Status != ProviderQuoteResponse.StatusEnum.Ready)
            {
                return;
            }

            var quote = _quoteStorage.GetQuoteWithLock(key);
            if (quote == null)
            {
                return;
            }

            if (quote.Status != Quote.QuoteStatus.WaitingProvidersQuotes)
            {
                return;
            }

            quote.ProviderQuoteResponses[providerId] = providerQuoteResponse;
            if (
                quote.ProviderQuoteRequests.Keys.All(key =>
                    quote.ProviderQuoteResponses.ContainsKey(key)
                )
            )
            {
                ProcessQuote(quote);
            }
        }
        finally
        {
            _quoteStorage.ReleaseQuote(key);
        }
    }

    private void ProcessQuote(Quote quote)
    {
        try
        {
            var priceCalculationResult = _priceCalculationService.CalculateAllProviderPrices(quote);

            quote.PriceCalculationResult = priceCalculationResult;
            var maxPriceToAccept = quote.AutoApprove
                ? quote.MaxPriceForAutoApprove
                : (decimal?)null;

            PopulateProviderBuyOrderRequestsAndPrice(quote, maxPriceToAccept);

            if (quote.AutoApprove)
            {
                ProcessQuoteWithAutoApprove(quote);
            }
            else
            {
                if (quote.QuantityToBuy > 0)
                {
                    quote.Status = Quote.QuoteStatus.WaitingAcceptance;
                    _eventSender.SendQuoteResponse(
                        quote.ToQuoteResponse(QuoteResponseStatusEnum.WaitingAcceptance)
                    );
                }
                else
                {
                    quote.Status = Quote.QuoteStatus.NoInventory;
                    _eventSender.SendQuoteResponse(
                        quote.ToQuoteResponse(QuoteResponseStatusEnum.NoInventory)
                    );
                }
            }
        }
        finally
        {
            _quoteStorage.ReleaseQuote(quote.Key);
        }
    }

    private void ProcessQuoteWithAutoApprove(Quote quote)
    {
        if (
            quote.MaxPriceForAutoApprove >= quote.Price
            && (
                quote.AllowPartial
                    ? quote.QuantityToBuy > 0
                    : quote.QuantityToBuy == quote.RequestedQuantity
            )
        )
        {
            quote.Status = Quote.QuoteStatus.WaitingAcceptance;
            _eventSender.SendQuoteResponse(
                quote.ToQuoteResponse(QuoteResponseStatusEnum.AutoAccepted)
            );
            AcceptLocateInternal(quote, null);
        }
        else
        {
            if (quote.PriceCalculationResult?.Offers.Any(x => x.Quantity > 0) == true)
            {
                quote.Status = Quote.QuoteStatus.Cancelled;
                _eventSender.SendQuoteResponse(
                    quote.ToQuoteResponse(QuoteResponseStatusEnum.AutoRejected)
                );
            }
            else
            {
                quote.Status = Quote.QuoteStatus.NoInventory;
                _eventSender.SendQuoteResponse(
                    quote.ToQuoteResponse(QuoteResponseStatusEnum.NoInventory)
                );
            }
        }
    }

    #endregion

    #region Aceept/Cancel Quote

    private bool CancelQuoteInternal(Quote quote, bool isTimeout)
    {
        if (
            quote.Status != Quote.QuoteStatus.WaitingAcceptance
            && quote.Status != Quote.QuoteStatus.WaitingProvidersQuotes
        )
        {
            var message =
                $"Attempt to cancel quote {quote.Id}, which is in the wrong state {quote.Status}";
            SendQuoteResponse(quote, QuoteResponseStatusEnum.RejectedBadRequest, message);
            return false;
        }

        quote.Status = Quote.QuoteStatus.Cancelled;
        var errorMessage = $"Cancelled by {(isTimeout ? "timeout" : "user")}";
        SendQuoteResponse(quote, QuoteResponseStatusEnum.Cancelled, errorMessage);
        return true;
    }

    public void AcceptQuote(QuoteRequest quoteRequest)
    {
        var key = new Quote.QuoteKey(quoteRequest.Id, quoteRequest.AccountId);
        try
        {
            var quote = _quoteStorage.GetQuoteWithLock(key);
            if (quote == null)
            {
                var message = $"Attempt to Accept not existing or expired quote {quoteRequest.Id}";
                SendQuoteResponse(
                    quoteRequest,
                    QuoteResponseStatusEnum.RejectedBadRequest,
                    message
                );
                return;
            }

            AcceptLocateInternal(quote, null);
        }
        finally
        {
            _quoteStorage.ReleaseQuote(key);
        }
    }

    public void CancelQuote(QuoteRequest quoteRequest)
    {
        var key = new Quote.QuoteKey(quoteRequest.Id, quoteRequest.AccountId);
        try
        {
            var quote = _quoteStorage.GetQuoteWithLock(key);
            if (quote == null)
            {
                var message = $"Attempt to cancel not existing or expired quote {quoteRequest.Id}";
                SendQuoteResponse(
                    quoteRequest,
                    QuoteResponseStatusEnum.RejectedBadRequest,
                    message
                );
                return;
            }

            CancelQuoteInternal(quote, false);
        }
        finally
        {
            _quoteStorage.ReleaseQuote(key);
        }
    }

    private bool AcceptLocateInternal(Quote quote, AcceptQuoteRequest? acceptQuoteRequest)
    {
        if (quote.Status != Quote.QuoteStatus.WaitingAcceptance)
        {
            var message =
                $"Attempt to Accept  quote {quote.Id}, which is in the wrong state {quote.Status}";
            SendQuoteResponse(quote, QuoteResponseStatusEnum.RejectedBadRequest, message);
            return true;
        }

        quote.Status = Quote.QuoteStatus.Accepted;
        if (acceptQuoteRequest?.MaxPriceToAccept != null)
        {
            PopulateProviderBuyOrderRequestsAndPrice(quote, acceptQuoteRequest.MaxPriceToAccept);
        }

        foreach (
            (
                _,
                (string provider, ProviderBuyRequest orderRequest)
            ) in quote.ProviderBuyOrderRequests
        )
        {
            var providerSetting = _providerSettingStorage.Get(provider);
            if (providerSetting == null || providerSetting.BuyRequestTopic == null)
            {
                quote.ProviderBuyOrderResponses.Add(
                    orderRequest.Id,
                    (
                        provider,
                        new ProviderBuyResponse()
                        {
                            AccountId = quote.AccountId,
                            BoughtAssets = [],
                            Id = orderRequest.Id,
                            QuoteId = quote.Id,
                            Symbol = quote.Symbol,
                            Status = ProviderBuyResponse.StatusEnum.Failed,
                        }
                    )
                );
                continue;
            }

            orderRequest.ValidTill = DateTime.UtcNow.Add(
                _timeoutOptions.Value.MaxProviderBuyWait / 2
            );

            _eventSender.SendProviderBuyOrderRequest(providerSetting.BuyRequestTopic, orderRequest);
        }

        return false;
    }

    private void PopulateProviderBuyOrderRequestsAndPrice(Quote quote, decimal? maxPriceToAccept)
    {
        var priceCalculationResult =
            quote.PriceCalculationResult
            ?? throw new InvalidOperationException(
                $"{nameof(quote.PriceCalculationResult)} is null, but {nameof(PopulateProviderBuyOrderRequestsAndPrice)} is called"
            );

        var result = _priceCalculationService.PopulateProviderBuyOrderRequestsAndPrice(
            priceCalculationResult,
            false,
            maxPriceToAccept
        );

        quote.Price = result.Price;
        quote.QuantityToBuy = result.QuantityToBuy;
        quote.ProviderBuyOrderRequests.Clear();

        foreach (var (key, value) in result.ProviderBuyOrderRequests)
        {
            quote.ProviderBuyOrderRequests.Add(key, value);
        }
    }

    #endregion

    #region Process Accepted Quote

    public void ProcessProviderBuyOrderResponse(
        ProviderBuyResponse providerBuyOrderResponse,
        string providerId
    )
    {
        var key = new Quote.QuoteKey(
            providerBuyOrderResponse.QuoteId,
            providerBuyOrderResponse.AccountId
        );

        try
        {
            var quote = _quoteStorage.GetQuoteWithLock(key);

            if (quote is not { Status: Quote.QuoteStatus.Accepted })
            {
                var warnMessage =
                    quote == null
                        ? $"Attempt to AddProviderBuyOrderResponse to not existing or expired quote {key}."
                        : $"Attempt to AddProviderBuyOrderResponse to quote {key}, which is in the wrong state {quote.Status}";

                if (
                    providerBuyOrderResponse.Status
                    is ProviderBuyResponse.StatusEnum.Fulfilled
                        or ProviderBuyResponse.StatusEnum.Partial
                )
                {
                    AddProviderBuyOrderResponseToInternalInventory(
                        providerBuyOrderResponse,
                        providerId,
                        CreatingType.Overbuy,
                        true
                    );
                }

                return;
            }

            quote.ProviderBuyOrderResponses.Add(
                providerBuyOrderResponse.Id,
                (providerId, providerBuyOrderResponse)
            );

            if (
                quote.ProviderBuyOrderRequests.Keys.All(x =>
                    quote.ProviderBuyOrderResponses.ContainsKey(x)
                )
                || quote.ActualQuantity >= quote.QuantityToBuy
            )
            {
                FinalizeTheQuote(quote);
            }
        }
        finally
        {
            _quoteStorage.ReleaseQuote(key);
        }
    }

    private void FinalizeTheQuote(Quote quote)
    {
        if (quote.QuantityToBuy > quote.ActualQuantity)
        {
            if (TryFindAdditionalQuantity(quote))
            {
                //If we found additional quantity, there is no need for finalization of the quote
                return;
            }
        }

        var status =
            quote.ActualQuantity == quote.RequestedQuantity ? QuoteResponseStatusEnum.Filled
            : quote.ActualQuantity > 0
            && (quote.AllowPartial || quote.QuantityToBuy == quote.ActualQuantity)
                ? QuoteResponseStatusEnum.Partial
            : QuoteResponseStatusEnum.NoInventory;

        quote.Status =
            status == QuoteResponseStatusEnum.NoInventory
                ? Quote.QuoteStatus.NoInventory
                : Quote.QuoteStatus.Filled;

        AddInventory(quote);

        // todo: simplify mapping
        var quoteResponse = new QuoteResponse
        {
            DetailsJson = quote.ToResponseDetailsJson(),
            Id = quote.Id,
            Price = quote.Price,
            ReqQty = quote.RequestedQuantity,
            FillQty =
                quote.Status == Quote.QuoteStatus.NoInventory
                    ? 0
                    : Math.Min(quote.ActualQuantity, quote.RequestedQuantity), //We bought more than required, user shouldn't know
            Status = status,
            Symbol = quote.Symbol,
            AccountId = quote.AccountId,
            // TODO_Review: I wonder whether this can be improved
            // todo: extract sources calculation logic
            Sources =
                quote.Status == Quote.QuoteStatus.NoInventory
                    ? Array.Empty<QuoteSourceInfo>()
                    : quote
                        .ProviderBuyOrderResponses.Values.Where(x =>
                            x.response.Status
                                is ProviderBuyResponse.StatusEnum.Fulfilled
                                    or ProviderBuyResponse.StatusEnum.Partial
                        )
                        .SelectMany(x =>
                            x.response.BoughtAssets.Select(priceInfo => (x.provider, priceInfo))
                        )
                        .Select(x =>
                        {
                            var provider = x.provider;
                            var priceInfo = x.priceInfo;

                            decimal discountedPrice = _priceCalculationService.GetDiscountedPrice(
                                priceInfo
                            );

                            return new QuoteSourceInfo
                            {
                                Provider = provider,
                                Source = priceInfo.Source!,
                                Price = priceInfo.Price,
                                Qty = priceInfo.Quantity,
                                DiscountedPrice = discountedPrice,
                            };
                        })
                        .ToArray(),
        };

        SendNotificationIfNegativeProfit(quoteResponse);

        _eventSender.SendQuoteResponse(quoteResponse);
    }

    private bool TryFindAdditionalQuantity(Quote quote)
    {
        var newBuyOrderRequests = FindProvidersWithAdditionalQuantity(quote);

        foreach (var (destination, request) in newBuyOrderRequests)
        {
            quote.ProviderBuyOrderRequests.Add(request.Id, (destination, request));
            var providerSettings = _providerSettingStorage.Get(destination);

            if (providerSettings?.BuyRequestTopic == null)
            {
                quote.ProviderBuyOrderResponses.Add(
                    request.Id,
                    (
                        destination,
                        new ProviderBuyResponse()
                        {
                            Id = request.Id,
                            AccountId = quote.AccountId,
                            BoughtAssets = [],
                            QuoteId = quote.Id,
                            Symbol = quote.Symbol,
                            Status = ProviderBuyResponse.StatusEnum.Failed,
                        }
                    )
                );
                continue;
            }

            request.ValidTill = DateTime.UtcNow.Add(_timeoutOptions.Value.MaxProviderBuyWait / 2);
            _eventSender.SendProviderBuyOrderRequest(providerSettings.BuyRequestTopic, request);
            quote.Status = Quote.QuoteStatus.Accepted; // to change last status updated time
        }

        return newBuyOrderRequests.Any();
    }

    private Dictionary<string, ProviderBuyRequest> FindProvidersWithAdditionalQuantity(Quote quote)
    {
        //TODO: shouldn't we store actual copy of all inventories??
        var requiredQuantity = quote.QuantityToBuy - quote.ActualQuantity;
        var discountedPrices = quote.PriceCalculationResult?.Offers ?? Array.Empty<ProviderOffer>();

        var newBuyOrderRequests = new Dictionary<string, ProviderBuyRequest>();
        foreach (
            var discountedPrice in discountedPrices
                .Where(x => x.DiscountedPrice <= quote.Price)
                .OrderBy(x => x.DiscountedPrice)
        )
        {
            if (requiredQuantity <= 0)
            {
                break;
            }

            if (
                quote
                    .ProviderBuyOrderRequests.Keys.Where(x =>
                        quote.ProviderBuyOrderRequests[x].provider == discountedPrice.Provider
                    )
                    .Any(x =>
                        !quote.ProviderBuyOrderResponses.ContainsKey(x)
                        || quote.ProviderBuyOrderResponses[x].response.Status
                            != ProviderBuyResponse.StatusEnum.Fulfilled
                    )
            )
            {
                //it means we have a request to current destination that wasn't responded or not fully covered
                continue;
            }

            //TODO: Improvement: seems like we will sometimes have additional iteration of buying from same destination if in this destination we already bought all inventory, but it shouldn't be very often
            var q = Math.Min(requiredQuantity, discountedPrice.Quantity);
            if (q == 0)
            {
                continue;
            }

            if (!newBuyOrderRequests.TryGetValue(discountedPrice.Provider, out var request))
            {
                request = quote.ToProviderBuyRequest();
                newBuyOrderRequests.Add(discountedPrice.Provider, request);
            }

            request.RequestedAssets.Add(
                new PriceInfo()
                {
                    Price = discountedPrice.Price,
                    Quantity = q,
                    Source = discountedPrice.Source,
                }
            );
            requiredQuantity -= q;
        }

        return newBuyOrderRequests;
    }

    private void AddInventory(Quote quote)
    {
        var remainingQuantity = quote.QuantityToBuy;
        var additionalQuantity = 0;
        foreach (
            var (destinationId, destinationBuyOrderResponse) in quote
                .ProviderBuyOrderResponses
                .Values
        )
        {
            if (
                destinationBuyOrderResponse.Status
                is ProviderBuyResponse.StatusEnum.Fulfilled
                    or ProviderBuyResponse.StatusEnum.Partial
            )
            {
                foreach (var boughtItem in destinationBuyOrderResponse.BoughtAssets)
                {
                    var discountedPrice = quote.PriceCalculationResult?.Offers.FirstOrDefault(x =>
                        x.Source == boughtItem.Source
                        && x.Price == boughtItem.Price
                        && String.Equals(
                            x.Provider,
                            destinationId,
                            StringComparison.OrdinalIgnoreCase
                        )
                    );

                    var q = Math.Min(remainingQuantity, boughtItem.Quantity);
                    additionalQuantity += boughtItem.Quantity - q;
                    if (q == 0)
                    {
                        continue;
                    }

                    _inventoryService.AddLocates(
                        quote.AccountId,
                        destinationBuyOrderResponse.Symbol,
                        q,
                        boughtItem.Price,
                        (boughtItem.Source) ?? destinationId
                    );
                }
            }
        }
    }

    private void AddFakeQuoteForInventoryAddedToInternal(
        AddInternalInventoryItemRequest request,
        string destinationId,
        PriceInfo item
    )
    {
        var quoteResponse = new QuoteResponse()
        {
            Id = Guid.NewGuid().ToString(),
            Price = request.Price,
            Status = QuoteResponseStatusEnum.Filled,
            Symbol = request.Symbol,
            AccountId = "admin_overbuy",
            FillQty = request.Quantity,
            ReqQty = request.Quantity,
            Sources = new QuoteSourceInfo[]
            {
                new QuoteSourceInfo()
                {
                    Price = request.Price,
                    Provider = destinationId,
                    Qty = request.Quantity,
                    Source = request.Source,
                    DiscountedPrice = _priceCalculationService.GetDiscountedPrice(item),
                },
            },
        };
        _eventSender.SendQuoteResponse(quoteResponse);
    }

    private void AddProviderBuyOrderResponseToInternalInventory(
        ProviderBuyResponse destinationBuyOrderResponse,
        string provider,
        CreatingType addingType,
        bool createFakeQuote = false
    )
    {
        foreach (var item in destinationBuyOrderResponse.BoughtAssets)
        {
            var request = new AddInternalInventoryItemRequest()
            {
                Symbol = destinationBuyOrderResponse.Symbol,
                Source = item.Source,
                Price = item.Price,
                Quantity = item.Quantity,
                CreatingType = addingType,
            };
            _eventSender.SendAddInternalInventoryItemRequest(request);

            if (createFakeQuote)
            {
                AddFakeQuoteForInventoryAddedToInternal(request, provider, item);
            }
        }
    }

    private void SendNotificationIfNegativeProfit(QuoteResponse quoteResponse)
    {
        if (
            quoteResponse.Status
            is QuoteResponseStatusEnum.Filled
                or QuoteResponseStatusEnum.Partial
        )
        {
            var locatorQuoteResponse = LocatorQuoteResponse.From(quoteResponse);

            if (locatorQuoteResponse.DiscountedPrice > locatorQuoteResponse.Price)
            {
                _notificationService.Add(
                    new NotificationEvent(
                        Type: NotificationType.Warning,
                        Kind: LocatorErrorKind.NegativeProfitQuote.ToString(),
                        GroupParameters: quoteResponse.Id,
                        Time: DateTime.UtcNow,
                        Message: $"Locates were sold with negative profit. QuoteId: {quoteResponse.Id}"
                    )
                );
            }
        }
    }

    #endregion

    #region Process Hanging Quotes

    public void ProcessHangingQuotes()
    {
        ProcessHangingQuoteResponseWaitingQuotes();
        ProcessHangingBuyResponseWaitingQuotes();
        ProcessHangingAcceptQuote();
        RemoveHistory();
    }

    private void RemoveHistory()
    {
        var time = DateTime.UtcNow.Subtract(_timeoutOptions.Value.RemoveHistoryTimeout);
        var quotes = _quoteStorage.GetNotActiveQuotes(time);

        if (quotes.Length == 0)
        {
            return;
        }

        foreach (var q in quotes)
        {
            try
            {
                var quote = _quoteStorage.TryGetQuoteWithLock(q.Key);
                if (quote == null || quote.LastStatusUpdate > time)
                {
                    continue;
                }

                _quoteStorage.DeleteQuote(quote);
            }
            finally
            {
                _quoteStorage.ReleaseQuote(q.Key);
            }
        }
    }

    private void ProcessHangingAcceptQuote()
    {
        var quotes = _quoteStorage.GetQuotes(
            Quote.QuoteStatus.WaitingAcceptance,
            DateTime.UtcNow.Subtract(_timeoutOptions.Value.MaxQuoteAcceptWait)
        );

        if (quotes.Length == 0)
        {
            return;
        }

        foreach (var q in quotes)
        {
            try
            {
                var quote = _quoteStorage.TryGetQuoteWithLock(q.Key);
                if (quote is not { Status: Quote.QuoteStatus.WaitingAcceptance })
                {
                    continue;
                }

                CancelQuoteInternal(quote, isTimeout: true);
            }
            finally
            {
                _quoteStorage.ReleaseQuote(q.Key);
            }
        }
    }

    private void ProcessHangingBuyResponseWaitingQuotes()
    {
        var quotes = _quoteStorage.GetQuotes(
            Quote.QuoteStatus.Accepted,
            DateTime.UtcNow.Subtract(_timeoutOptions.Value.MaxProviderBuyWait)
        );

        if (quotes.Length == 0)
        {
            return;
        }

        foreach (var q in quotes)
        {
            try
            {
                var quote = _quoteStorage.TryGetQuoteWithLock(q.Key);
                if (quote is not { Status: Quote.QuoteStatus.Accepted })
                {
                    continue;
                }

                FinalizeTheQuote(quote);
            }
            finally
            {
                _quoteStorage.ReleaseQuote(q.Key);
            }
        }
    }

    private void ProcessHangingQuoteResponseWaitingQuotes()
    {
        var quotes = _quoteStorage.GetQuotes(
            Quote.QuoteStatus.WaitingProvidersQuotes,
            DateTime.UtcNow.Subtract(_timeoutOptions.Value.MaxProviderQuoteWait)
        );

        if (quotes.Length == 0)
        {
            return;
        }

        foreach (var q in quotes)
        {
            try
            {
                var quote = _quoteStorage.TryGetQuoteWithLock(q.Key);
                if (quote is not { Status: Quote.QuoteStatus.WaitingProvidersQuotes })
                {
                    continue;
                }

                quote.WaitingProvidersTimeout = true;
                ProcessQuote(quote);
            }
            finally
            {
                _quoteStorage.ReleaseQuote(q.Key);
            }
        }
    }

    #endregion

    private void SendQuoteResponse(
        QuoteRequest quoteRequest,
        QuoteResponseStatusEnum status,
        string? message
    )
    {
        QuoteResponse quoteResponse = quoteRequest.ToQuoteResponse(status, message);
        _eventSender.SendQuoteResponse(quoteResponse);
    }

    private void SendQuoteResponse(
        Quote quote,
        QuoteResponseStatusEnum status,
        string? errorMessage
    )
    {
        QuoteResponse quoteResponse = quote.ToQuoteResponse(status, errorMessage);
        _eventSender.SendQuoteResponse(quoteResponse);
    }
}
