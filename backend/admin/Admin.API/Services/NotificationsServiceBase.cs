using Newtonsoft.Json.Converters;
using Admin.API.Services.Interfaces;
using Lib.AspNetCore.ServerSentEvents;
using Newtonsoft.Json;
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

    private string SerializeObject<T>(T value)
    {
        JsonSerializerSettings _serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings();
        _serializerSettings.Converters.Add(new StringEnumConverter());
        return JsonConvert.SerializeObject(value, _serializerSettings);
    }

    public void SendLocateRequest(LocateRequestModel data)
    {
        using var activity = TracingConfiguration.StartActivity("NotificationsService SendLocateRequest");
        try
        {
            Send(Constants.SSEMethods.LocateRequest,
                data);
        }
        catch (Exception e)
        {
            activity?.LogException(e);
            throw;
        }
    }

    private void Send(string method, object data)
    {
        var serializedData = SerializeObject(data);
        List<Task> tasks = new List<Task>
        {
            SendToGroupAsync(Constants.KnownRoles.Admin, method, serializedData),
            SendToGroupAsync(Constants.KnownRoles.Viewer, method, serializedData)
        };

        Task.WhenAll(tasks).GetAwaiter().GetResult();
    }

    private Task SendToGroupAsync(string groupName, string type, params string[] data)
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
            Send(Constants.SSEMethods.Locate, data);
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
            SendToGroupAsync(Constants.KnownRoles.Admin, Constants.SSEMethods.Notification,
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
                Type = Constants.SSEMethods.InternalInventory,
                Data = [SerializeObject(message)]
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
            var method = Constants.SSEMethods.LocateRequestHistory;
            var serializedData = SerializeObject(historyItems);
            List<Task> tasks =
            [
                SendToGroupAsync(Constants.KnownRoles.Admin, method, serializedData),
                SendToGroupAsync(Constants.KnownRoles.Viewer, method, serializedData)
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
            var method = Constants.SSEMethods.LocateHistory;
            var serializedData = SerializeObject(historyItems);
            List<Task> tasks =
            [
                SendToGroupAsync(Constants.KnownRoles.Admin, method, serializedData),
                SendToGroupAsync(Constants.KnownRoles.Viewer, method, serializedData)
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