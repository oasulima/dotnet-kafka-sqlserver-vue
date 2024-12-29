using Newtonsoft.Json.Converters;
using Admin.API.Services.Interfaces;
using Lib.AspNetCore.ServerSentEvents;
using Newtonsoft.Json;
using Admin.API.Models;
using Admin.API.Models.Cache;
using Shared;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

namespace Admin.API.Services;

public class NotificationsServiceBase : INotificationsServiceBase
{
    #region Fields

    private INotificationsServerSentEventsService _notificationsServerSentEventsService;

    #endregion

    #region Constructor

    public NotificationsServiceBase(INotificationsServerSentEventsService notificationsServerSentEventsService)
    {
        _notificationsServerSentEventsService = notificationsServerSentEventsService;
    }

    #endregion

    #region Methods

    //protected Task SendSseEventAsync(string notification, bool alert)
    //{
    //    return _notificationsServerSentEventsService.SendEventAsync(new ServerSentEvent
    //    {
    //        Type = alert ? "alert" : null,
    //        Data = new List<string>(notification.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
    //    });
    //}

    private string[] SerializeObject<T>(T value)
    {
        JsonSerializerSettings _serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();
        _serializerSettings.Converters.Add(new StringEnumConverter());
        return new string[] { JsonConvert.SerializeObject(value, _serializerSettings) };
    }

    public void SendLocateRequest(LocateRequestModel data)
    {
        using var activity = TracingConfiguration.StartActivity("NotificationsService SendLocateRequest");
        try
        {
            Send(data.SourceDetails.Select(x => x.Source).Distinct(), Constants.SignalRMethods.LocateRequest,
                data);
        }
        catch (Exception e)
        {
            activity?.LogException(e);
            throw;
        }
    }

    private void Send(IEnumerable<string> providerIds, string method, object data)
    {
        List<Task> tasks = new List<Task>
        {
            SendToGroupAsync(Constants.KnownRoles.Admin, method, SerializeObject(data)),
            SendToGroupAsync(Constants.KnownRoles.Viewer, method, SerializeObject(data))
        };

        Task.WhenAll(tasks).GetAwaiter().GetResult();
    }

    private Task SendToGroupAsync(string groupName, string type, IList<string> data)
        => _notificationsServerSentEventsService.SendEventAsync(
            groupName,
            new ServerSentEvent
            {
                Type = type,
                Data = data
            });

    public void SendLocate(LocateModel data)
    {
        using var activity = TracingConfiguration.StartActivity("NotificationsService SendLocate");
        try
        {
            Send(data.SourceDetails.Select(x => x.Source).Distinct(), Constants.SignalRMethods.Locate, data);
        }
        catch (Exception e)
        {
            activity?.LogException(e);
            throw;
        }
    }

    public void SendNotifications(GroupedNotification[] notifications)
    {
        using var activity = TracingConfiguration.StartActivity("NotificationsService SendNotifications");
        try
        {
            SendToGroupAsync(Constants.KnownRoles.Admin, Constants.SignalRMethods.Notification,
                SerializeObject(notifications)).GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            activity?.LogException(e);
            throw;
        }
    }

    public void SendInternalInventoryItem(InternalInventoryItem message)
    {
        using var activity = TracingConfiguration.StartActivity("NotificationsService SendInternalInventoryItem");
        try
        {
            _notificationsServerSentEventsService.SendEventAsync(new ServerSentEvent
            {
                Type = Constants.SignalRMethods.InternalInventory,
                Data = SerializeObject(message)
            }).GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            activity?.LogException(e);
            throw;
        }
    }

    public void SendHistory(IList<LocateRequestModel> historyItems)
    {
        using var activity = TracingConfiguration.StartActivity("NotificationsService SendHistory Locate Requests");
        try
        {
            var method = Constants.SignalRMethods.LocateRequestHistory;

            List<Task> tasks =
            [
                SendToGroupAsync(Constants.KnownRoles.Admin, method, SerializeObject(historyItems)),
                SendToGroupAsync(Constants.KnownRoles.Viewer, method, SerializeObject(historyItems))
            ];

            Task.WhenAll(tasks).GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            activity?.LogException(e);
            throw;
        }
    }

    public void SendHistory(IList<LocateModel> historyItems)
    {
        using var activity = TracingConfiguration.StartActivity("NotificationsService SendHistory Locates");
        try
        {
            var method = Constants.SignalRMethods.LocateHistory;

            List<Task> tasks =
            [
                SendToGroupAsync(Constants.KnownRoles.Admin, method, SerializeObject(historyItems)),
                SendToGroupAsync(Constants.KnownRoles.Viewer, method, SerializeObject(historyItems))
            ];

            Task.WhenAll(tasks).GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            activity?.LogException(e);
            throw;
        }
    }

    #endregion
}