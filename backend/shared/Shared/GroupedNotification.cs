using System;

namespace Shared;

public enum NotificationType
{
    Warning,
    Error,
    Critical,
}

public record GroupedNotification(NotificationType Type, string Kind, string GroupParameters, string LastMessage, DateTime FirstTime, DateTime LastTime, int Count);
