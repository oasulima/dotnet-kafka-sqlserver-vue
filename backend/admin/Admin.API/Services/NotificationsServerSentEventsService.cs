using Admin.API.Services.Interfaces;
using Lib.AspNetCore.ServerSentEvents;
using Microsoft.Extensions.Options;
using Shared;

namespace Admin.API.Services;

public class NotificationsServerSentEventsService
    : ServerSentEventsService,
        INotificationsServerSentEventsService
{
    private readonly ILocateRequestsCache _locateRequestsCache;
    private readonly ILocatesCache _locatesCache;

    private const int HistoryChunkSize = 2_000;

    public NotificationsServerSentEventsService(
        ILocateRequestsCache locateRequestsCache,
        ILocatesCache locatesCache,
        IOptions<ServerSentEventsServiceOptions<NotificationsServerSentEventsService>> options
    )
        : base(options.ToBaseServerSentEventsServiceOptions())
    {
        _locateRequestsCache = locateRequestsCache;
        _locatesCache = locatesCache;
        ClientConnected += OnClientConnected;
    }

    private void OnClientConnected(object? sender, ServerSentEventsClientConnectedArgs args)
    {
        using var activity = TracingConfiguration.StartActivity(
            "Notifications SSE OnClientConnected"
        );
        try
        {
            AddUserToProviderIdGroup(args.Client);

            SendHistory(args.Client).GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            activity?.LogException(e);
            throw;
        }
    }

    private void AddUserToProviderIdGroup(IServerSentEventsClient client)
    {
        string groupName = Constants.KnownRoles.Admin;
        AddToGroup(groupName, client);
    }

    private Task SendHistory(IServerSentEventsClient client)
    {
        var locateRequests = _locateRequestsCache.GetHistoryRecords();
        var locates = _locatesCache.GetHistoryRecords();

        var locateRequestTask = SendHistoryAsync(
            client,
            Constants.SSEMethods.LocateRequestHistory,
            locateRequests
        );
        var locateTask = SendHistoryAsync(client, Constants.SSEMethods.LocateHistory, locates);

        return Task.WhenAll(locateRequestTask, locateTask);
    }

    private async Task SendHistoryAsync<T>(
        IServerSentEventsClient client,
        string method,
        IEnumerable<T> historyItems
    )
    {
        foreach (var chunk in historyItems.Chunk(HistoryChunkSize))
        {
            await client.SendEventAsync(
                new ServerSentEvent() { Type = method, Data = [Converter.Serialize(chunk)] }
            );
        }
    }
}
