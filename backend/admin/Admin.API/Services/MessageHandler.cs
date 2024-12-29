using System.Collections.Concurrent;
using Refit;
using Admin.API.Models.Cache;
using Admin.API.Services.Interfaces;
using Shared;
using Shared.Quote;
using Shared.Refit;

namespace Admin.API.Services;

public class MessageHandler : IMessageHandler
{
    private readonly ILocateRequestsCache _locateRequestsCache;
    private readonly ILocatesCache _locatesCache;
    private readonly INotificationsServiceBase _adminUiHubMethods;
    private readonly ITimeService _timeService;

    private readonly IReportingApi reportingApi;

    private const int RestoreData_Take = 2_000;
    private Task? restoringDataTask;

    public MessageHandler(
        ILocateRequestsCache locateRequestsCache,
        ILocatesCache locatesCache,
        INotificationsServiceBase adminUiHubMethods,
        ITimeService timeService,
        IReportingApi reportingApi)
    {
        _locateRequestsCache = locateRequestsCache;
        _locatesCache = locatesCache;
        _adminUiHubMethods = adminUiHubMethods;
        _timeService = timeService;
        this.reportingApi = reportingApi;

        restoringDataTask = RestoreData();
    }

    private readonly CancellationTokenSource _expressRestoringStopCts = new();

    private async Task RestoreData()
    {
        using var activity = TracingConfiguration.StartActivity("MessageHandler.RestoreData");
        try
        {
            ConcurrentQueue<(List<LocateRequestModel> locateRequests, List<LocateModel> locates)> handledResponsesToSend =
        new();
            var utcNow = DateTime.UtcNow;
            var from = _timeService.GetPreviousCleanupTimeInUtc(utcNow);

            var skip = 0;

            var handlingResponsesCts = new CancellationTokenSource();

            var sendingResponsesTask = RestoreData_SendHandledResponses(handlingResponsesCts.Token, handledResponsesToSend);

            while (!_expressRestoringStopCts.IsCancellationRequested)
            {

                var responses =
                    await reportingApi.GetLocatorQuoteResponses(from, utcNow, RestoreData_Take, skip);



                if (responses.Count == 0) break;

                RestoreData_HandleResponses(responses, handledResponsesToSend);
                skip += RestoreData_Take;
            }

            handlingResponsesCts.Cancel();

            if (_expressRestoringStopCts.IsCancellationRequested)
            {
                handledResponsesToSend.Clear();
            }

            try
            {
                await sendingResponsesTask;
            }
            catch (TaskCanceledException)
            {
            }
        }
        catch (Exception e)
        {
            activity?.LogException(e);
            throw;
        }
    }

    public void CleanData()
    {
        _expressRestoringStopCts.Cancel();
        try
        {
            restoringDataTask?.GetAwaiter().GetResult();
        }
        catch (TaskCanceledException e)
        {
        }

        _locatesCache.Clear();
        _locateRequestsCache.Clear();
    }

    private void RestoreData_HandleResponses(IList<LocatorQuoteResponse> responses,
        ConcurrentQueue<(List<LocateRequestModel> locateRequests, List<LocateModel> locates)> handledResponsesToSend)
    {
        List<LocateRequestModel> locateRequests = new List<LocateRequestModel>(RestoreData_Take);
        List<LocateModel> locates = new List<LocateModel>(RestoreData_Take);
        foreach (var response in responses)
        {
            if (_expressRestoringStopCts.IsCancellationRequested) break;

            var (locateRequest, locate) = Handle(response);
            if (locateRequest != null)
            {
                locateRequests.Add(locateRequest);
            }

            if (locate != null)
            {
                locates.Add(locate);
            }
        }

        if (!_expressRestoringStopCts.IsCancellationRequested)
        {
            handledResponsesToSend.Enqueue((locateRequests, locates));
        }
    }

    private async Task RestoreData_SendHandledResponses(CancellationToken handlingResponsesCts,
        ConcurrentQueue<(List<LocateRequestModel> locateRequests, List<LocateModel> locates)> handledResponsesToSend)
    {
        while (!_expressRestoringStopCts.IsCancellationRequested && !handlingResponsesCts.IsCancellationRequested)
        {
            await Task.Delay(500, handlingResponsesCts);
            while (!_expressRestoringStopCts.IsCancellationRequested && !handledResponsesToSend.IsEmpty)
            {
                handledResponsesToSend.TryDequeue(out var data);
                var (locateRequests, locates) = data;

                if (!_expressRestoringStopCts.IsCancellationRequested && locateRequests.Count > 0)
                {
                    _adminUiHubMethods.SendHistory(locateRequests);
                }

                if (!_expressRestoringStopCts.IsCancellationRequested && locates.Count > 0)
                {
                    _adminUiHubMethods.SendHistory(locates);
                }
            }
        }
    }


    private (LocateRequestModel? locateRequest, LocateModel? locate) Handle(LocatorQuoteResponse quoteResponse)
    {
        LocateRequestModel? locateRequest = null;
        LocateModel? locate = null;

        if (quoteResponse.Status is QuoteResponseStatusEnum.WaitingAcceptance
            or QuoteResponseStatusEnum.AutoAccepted
            or QuoteResponseStatusEnum.AutoRejected
            or QuoteResponseStatusEnum.RejectedDuplicate
            or QuoteResponseStatusEnum.RejectedBadRequest
            or QuoteResponseStatusEnum.NoInventory)
        {
            locateRequest = _locateRequestsCache.Memorize(quoteResponse);
        }

        if (quoteResponse.Status is QuoteResponseStatusEnum.Cancelled
            or QuoteResponseStatusEnum.Expired
            or QuoteResponseStatusEnum.Failed
            or QuoteResponseStatusEnum.RejectedBadRequest
            or QuoteResponseStatusEnum.RejectedDuplicate
            or QuoteResponseStatusEnum.Partial
            or QuoteResponseStatusEnum.Filled
            or QuoteResponseStatusEnum.NoInventory
            or QuoteResponseStatusEnum.AutoRejected)
        {
            locate = _locatesCache.Memorize(quoteResponse);
        }

        return (locateRequest, locate);
    }

    public void Handle(QuoteResponse quoteResp)
    {
        var (locateRequest, locate) = Handle(new LocatorQuoteResponse(quoteResp));

        if (locateRequest != null)
        {
            _adminUiHubMethods.SendLocateRequest(locateRequest);
        }

        if (locate != null)
        {
            _adminUiHubMethods.SendLocate(locate);
        }
    }
}