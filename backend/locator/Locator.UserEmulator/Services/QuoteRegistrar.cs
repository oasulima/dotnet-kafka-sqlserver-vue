using System.Collections.Immutable;
using TradeZero.Locator.Emulator.Kafka;
using Shared;

namespace TradeZero.Locator.Emulator.Services;


public class QuoteRegistrar
{
    private readonly IKafkaSender _kafkaSender;

    public QuoteRegistrar(IKafkaSender kafkaSender)
    {
        _kafkaSender = kafkaSender;
    }

    public Task<QuoteResponse> QuoteRequestAsync(QuoteRequest quoteRequest)
    {
        var tcs = new TaskCompletionSource<QuoteResponse>();
        SharedData.Cache[quoteRequest.Id] = tcs;

        _kafkaSender.SendQuote(quoteRequest);

        return tcs.Task;
    }

    private readonly IImmutableSet<QuoteResponseStatusEnum> _quoteStatusesToRegister = QuoteResponseStatus.FinalizedStatuses
        .Add(QuoteResponseStatusEnum.WaitingAcceptance);

    public void RegisterQuoteResponse(QuoteResponse response)
    {
        SharedData.Log(
            $"RegisterQuoteResponse: {response.Id}: {response.Status}; symbol:{response.Symbol}; account:{response.AccountId}; Error:{response.ErrorMessage};");
        if (_quoteStatusesToRegister.Contains(response.Status)
           )
        {
            if (SharedData.Cache.Remove(response.Id, out var tcs))
            {
                tcs.SetResult(response);
            }
            else
            {
                SharedData.Log(
                    $"Couldn't remove: {response.Id}: {response.Status}; symbol:{response.Symbol}; account:{response.AccountId}",
                    false);
            }
        }
    }

    public Task<QuoteResponse> AcceptQuote(QuoteRequest quoteRequest)
    {
        var tcs = new TaskCompletionSource<QuoteResponse>();
        SharedData.Cache[quoteRequest.Id] = tcs;

        quoteRequest.RequestType = QuoteRequest.RequestTypeEnum.QuoteAccept;
        _kafkaSender.SendQuote(quoteRequest);

        return tcs.Task;
    }

    public Task<QuoteResponse> CancelQuote(QuoteRequest quoteRequest)
    {
        var tcs = new TaskCompletionSource<QuoteResponse>();
        SharedData.Cache[quoteRequest.Id] = tcs;

        quoteRequest.RequestType = QuoteRequest.RequestTypeEnum.QuoteCancel;
        _kafkaSender.SendQuote(quoteRequest);

        return tcs.Task;
    }

    public Task<QuoteResponse> IgnoreQuote(QuoteRequest quoteRequest)
    {
        var tcs = new TaskCompletionSource<QuoteResponse>();
        SharedData.Cache[quoteRequest.Id] = tcs;

        return tcs.Task;
    }
}