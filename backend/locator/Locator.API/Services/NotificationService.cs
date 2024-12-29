using System.Collections.Concurrent;
using Locator.API.Services.Interfaces;
using Locator.API.Utility;
using Shared;
using Shared.Locator;

namespace Locator.API.Services;

public class NotificationService : INotificationService
{
    private readonly object _syncRoot = new();
    private ConcurrentBag<NotificationEvent> _notificationBag = new();
    private readonly INotificationEventSender _notificationSender;

    public NotificationService(INotificationEventSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public void SendAll()
    {
        var items = new List<NotificationEvent>();

        while (_notificationBag.TryTake(out var item))
        {
            items.Add(item);
        }

        if (items.IsEmpty())
        {
            return;
        }

        var groups = items
            .GroupBy(x => (x.Type, x.Kind, x.GroupParameters))
            .Select(x =>
            {
                var key = x.Key;
                var group = x.OrderBy(x => x.Time).ToArray();
                var first = group.First();
                var last = group.Last();
                var count = group.Length;

                return new GroupedNotification(
                    Type: key.Type,
                    Kind: key.Kind,
                    GroupParameters: key.GroupParameters,
                    LastMessage: last.Message,
                    FirstTime: first.Time,
                    LastTime: last.Time,
                    Count: count
                );
            })
            .ToArray();

        _notificationSender.Send(groups);
    }

    public void Add(NotificationEvent notificationEvent)
    {
        _notificationBag.Add(notificationEvent);
    }
}
