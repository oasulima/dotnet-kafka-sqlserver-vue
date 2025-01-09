using Admin.API.Models.Cache;
using Shared;

namespace Admin.API.Services.Interfaces;

public interface INotificationsServiceBase
{
    void SendLocateRequest(LocateRequestModel data);
    void SendLocate(LocateModel data);
    void SendNotifications(GroupedNotification[] notification);
    void SendInternalInventoryItem(InternalInventoryItem message);
    void SendHistory(IList<LocateModel> historyItems);
    void SendHistory(IList<LocateRequestModel> historyItems);
}
