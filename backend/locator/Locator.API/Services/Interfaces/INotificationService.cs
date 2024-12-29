using Shared.Locator;

namespace Locator.API.Services.Interfaces;

public interface INotificationService
{
    void Add(NotificationEvent notificationEvent);
    void SendAll();
}
