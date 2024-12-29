namespace Locator.API.Services.Interfaces;

public interface ILocatorErrorReporter
{
    void Report(IEnumerable<LocatorError> errors, string scope, string? commonDetails = null);

    void Report(LocatorError error, string scope, string? moreDetails = null);
}