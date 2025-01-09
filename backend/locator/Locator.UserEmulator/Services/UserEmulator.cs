using Locator.UserEmulator;
using Locator.UserEmulator.Options;
using Locator.UserEmulator.Utility;
using Shared;

namespace Locator.UserEmulator.Services;

public class UserEmulator
{
    private readonly QuoteRegistrar _quoteRegistrar;
    private readonly RandomHelper _randomHelper;
    private readonly EmulatorOptions _emulatorOptions;
    private readonly int _senderIndex;

    public UserEmulator(
        QuoteRegistrar quoteRegistrar,
        RandomHelper randomHelper,
        int senderIndex,
        EmulatorOptions emulatorOptions
    )
    {
        _quoteRegistrar = quoteRegistrar;
        _randomHelper = randomHelper;
        _senderIndex = senderIndex;
        _emulatorOptions = emulatorOptions;
    }

    public async Task StartAsync(CancellationToken stop, CancellationToken allowNewQuotes)
    {
        var id = Guid.NewGuid();
        SharedData.Log($"Start {id}", false);
        try
        {
            while (!stop.IsCancellationRequested && !allowNewQuotes.IsCancellationRequested)
            {
                await Task.Delay(_randomHelper.GetDelayBeforeQuote(), allowNewQuotes);

                var quote = GetRandomQuote();

                SharedData.Log(
                    $"Before Quote: id:{quote.Id}; sym: {quote.Symbol}; reqqty:{quote.Quantity}; account:{quote.AccountId}"
                );
                var quoteResponse = await _quoteRegistrar.QuoteRequestAsync(quote);
                SharedData.Log(
                    $"After Quote: id:{quote.Id}; sym: {quote.Symbol}; reqqty:{quote.Quantity}; account:{quote.AccountId}"
                );

                SharedData.IncrementQuotes();

                if (quoteResponse.Status != QuoteResponseStatusEnum.WaitingAcceptance)
                {
                    SharedData.IncrementFailed();
                    continue;
                }

                if (_randomHelper.ShouldAccept())
                {
                    SharedData.Log(
                        $"Before Accept: id:{quote.Id}; symbol:{quote.Symbol}; account:{quote.AccountId}"
                    );
                    var acceptQuoteResponse = await _quoteRegistrar.AcceptQuote(quote);

                    SharedData.Log(
                        $"After Accept: id:{quote.Id}; fillqty:{acceptQuoteResponse.FillQty}; symbol:{acceptQuoteResponse.Symbol};price:{acceptQuoteResponse.Price: #.####}; sources:{string.Join("/", acceptQuoteResponse.Sources.Select(x => $"price:{x.Price: #.####}|qty:{x.Qty}|source:{x.Source}|price:{x.Price: #.####}"))}"
                    );
                    SharedData.IncrementAccepts();
                }
                else if (_randomHelper.ShouldCancel())
                {
                    SharedData.Log(
                        $"Before Cancel {quote.Id}; symbol:{quote.Symbol}; account:{quote.AccountId}"
                    );
                    await _quoteRegistrar.CancelQuote(quote);
                    SharedData.Log(
                        $"After Cancel {quote.Id}; symbol:{quote.Symbol}; account:{quote.AccountId}"
                    );
                    SharedData.IncrementCancels();
                    continue;
                }
                else
                {
                    SharedData.Log(
                        $"Before Ignore {quote.Id}; symbol:{quote.Symbol}; account:{quote.AccountId}"
                    );
                    await _quoteRegistrar.IgnoreQuote(quote);
                    SharedData.Log(
                        $"After Ignore {quote.Id}; symbol:{quote.Symbol}; account:{quote.AccountId}"
                    );
                    SharedData.IncrementIgnores();
                    continue;
                }

                if (allowNewQuotes.IsCancellationRequested)
                    continue;

                //if (_randomHelper.ShouldOpenShort())
                //{
                //    await Task.Delay(_randomHelper.GetDelayBeforeOpenShortOrder(), allowNewQuotes);
                //    SharedData.Log(
                //        $"Before OpenShort: AccountId:{quote.AccountId}; FirmId:{quote.FirmId}; Symbol:{quote.Symbol}");
                //    await _locatorApi.OpenShort(quote.AccountId, quote.FirmId, quote.Symbol);
                //    SharedData.Log(
                //        $"After OpenShort: AccountId:{quote.AccountId}; FirmId:{quote.FirmId}; Symbol:{quote.Symbol}");

                //    if (_randomHelper.ShouldCloseShort())
                //    {
                //        await Task.Delay(_randomHelper.GetDelayBeforeCloseShortMilliseconds(), allowNewQuotes);
                //        SharedData.Log(
                //            $"Before CloseShort: AccountId:{quote.AccountId}; FirmId:{quote.FirmId}; Symbol:{quote.Symbol}");
                //        await _locatorApi.CloseShort(quote.AccountId, quote.FirmId, quote.Symbol);
                //        SharedData.Log(
                //            $"After CloseShort: AccountId:{quote.AccountId}; FirmId:{quote.FirmId}; Symbol:{quote.Symbol}");
                //    }
                //}

                //if (_randomHelper.ShouldSellback())
                //{
                //    await Task.Delay(_randomHelper.GetDelayBeforeSellback(), allowNewQuotes);
                //    SharedData.Log(
                //        $"Before Sellback: AccountId:{quote.AccountId}; FirmId:{quote.FirmId}; Symbol:{quote.Symbol}");
                //    await _locatorApi.Sellback(quote.AccountId, quote.FirmId, quote.Symbol);
                //    SharedData.Log(
                //        $"After Sellback: AccountId:{quote.AccountId}; FirmId:{quote.FirmId}; Symbol:{quote.Symbol}");
                //}
            }
        }
        catch (TaskCanceledException)
        {
            //noop
        }

        SharedData.Log($"Cancel {id}", false);
    }

    private QuoteRequest GetRandomQuote()
    {
        return new QuoteRequest
        {
            Id = StringIdGenerator.GenerateId(),
            Symbol = _randomHelper.GetRandomSymbol(),
            AccountId = GetAccountId(),
            AllowPartial = false,
            AutoApprove = false,
            Quantity = _randomHelper.GetRandomQuoteQuantity(),
            MaxPriceForAutoApprove = int.MaxValue,
            RequestType = QuoteRequest.RequestTypeEnum.QuoteRequest,
            Time = DateTime.UtcNow,
        };
    }

    private string GetAccountId()
    {
        return _senderIndex < _emulatorOptions.NumberOfUniqueAccounts
            ? $"UE{_senderIndex}"
            : $"UE{_emulatorOptions.NumberOfUniqueAccounts}";
    }
}
