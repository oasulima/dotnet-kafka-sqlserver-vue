using System;

namespace Shared.Locator;

public record NotificationEvent(NotificationType Type, string Kind, string GroupParameters, DateTime Time, string Message);

