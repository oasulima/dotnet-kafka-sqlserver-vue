namespace Shared.Locator;

public record NotificationEvent(
    NotificationType Type,
    string Kind, // TODO: LocatorErrorKind
    string GroupParameters,
    DateTime Time,
    string Message
);
