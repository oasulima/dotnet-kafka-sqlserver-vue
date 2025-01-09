using Locator.API.Services.Interfaces;
using Shared;
using Shared.Locator;

namespace Locator.API.Services;

public class LocatorErrorReporter : ILocatorErrorReporter
{
    private readonly INotificationService _notificationService;

    public LocatorErrorReporter(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void Report(IEnumerable<LocatorError> errors, string scope, string? commonDetails = null)
    {
        foreach (var error in errors)
        {
            Report(error, scope, commonDetails);
        }
    }

    public void Report(LocatorError error, string scope, string? moreDetails = null)
    {
        PerfLog(error, scope, moreDetails);
        AdminUiLog(error, moreDetails);
    }

    private void AdminUiLog(LocatorError error, string? moreDetails)
    {
        var message = error.Kind switch
        {
            LocatorErrorKind.InternalInvSourceProviderNotProvided =>
                "Internal Inventory Source is not provided. "
                    + $"Please provide Source for Internal Inventory locates.",

            LocatorErrorKind.InternalInvSourceProviderNotFound =>
                "Internal Inventory Source is not configured. " + $"Please configure Source.",

            LocatorErrorKind.MinUserPriceViolation =>
                $"Quote Price is less then MinUserPrice. MinUserPrice applied.",
            _ => null,
        };

        var output = BuildOutput(message, error.Details, moreDetails);

        if (output != null)
        {
            _notificationService.Add(
                new NotificationEvent(
                    Type: NotificationType.Error,
                    Kind: error.Kind.ToString(),
                    GroupParameters: error.Details ?? "",
                    Time: DateTime.UtcNow,
                    Message: output
                )
            );
        }
    }

    private static void PerfLog(LocatorError error, string scope, string? moreDetails)
    {
        var details = error.Details;
        var (level, message) = error.Kind switch
        {
            LocatorErrorKind.InternalInvSourceProviderNotProvided => (
                PerfLogLevel.Warning,
                "Internal Inventory Source is not provided. Locates are skipped."
            ),
            LocatorErrorKind.InternalInvSourceProviderNotFound => (
                PerfLogLevel.Warning,
                "Internal Inventory Source is not found. Locates are skipped."
            ),
            LocatorErrorKind.RegularProviderProviderNotFound => (
                PerfLogLevel.Warning,
                "Provider is not found. Locates are not included in Quote price calculation."
            ),
            LocatorErrorKind.RegularProviderPriceIsNotPositive => (
                PerfLogLevel.Warning,
                "Provider Price should be greater then 0.  Locates are skipped."
            ),
            LocatorErrorKind.InternalInvProviderNotFound => (
                PerfLogLevel.Warning,
                "Internal inventory provider is not found. Locates for this Provider are skipped."
            ),
            LocatorErrorKind.MinUserPriceViolation => (
                PerfLogLevel.Error,
                $"Quote Price is less then MinUserPrice. MinUserPrice applied. {error.Details}, {moreDetails}"
            ),
            _ => (PerfLogLevel.None, null),
        };

        var output = BuildOutput(message, details, moreDetails);

        if (string.IsNullOrWhiteSpace(output))
        {
            return;
        }

        switch (level)
        {
            case PerfLogLevel.Warning:
                break;
            case PerfLogLevel.Error:
                break;
        }
    }

    private static string? BuildOutput(string? message, string? details, string? moreDetails)
    {
        return (message, details, moreDetails) switch
        {
            (not null, not null, not null) => $"{message} {details}, {moreDetails}",
            (not null, not null, null) => $"{message} {details}",
            (not null, null, not null) => $"{message} {moreDetails}",
            (not null, null, null) => message,
            _ => null,
        };
    }

    private enum PerfLogLevel
    {
        None,
        Warning,
        Error,
    }
}
