using Shared;

namespace Locator.API.Services.Interfaces;

public interface INotificationEventSender
{
    void Send(GroupedNotification[] notification);
}