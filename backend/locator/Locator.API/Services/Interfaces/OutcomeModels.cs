namespace Locator.API.Services.Interfaces;

public record LocatorError(LocatorErrorKind Kind, string? Details = null);

public record Outcome<TResult>(TResult Result, IReadOnlyCollection<LocatorError> Errors)
{
    public Outcome(TResult result)
        : this(result, Array.Empty<LocatorError>()) { }

    public Outcome(TResult result, LocatorError error)
        : this(result, new[] { error }) { }

    public TResult Unwrap(List<LocatorError> errorList)
    {
        errorList.AddRange(Errors);

        return Result;
    }
}

public enum LocatorErrorKind
{
    InternalInvSourceProviderNotProvided,
    InternalInvSourceProviderNotFound,
    RegularProviderProviderNotFound,
    RegularProviderPriceIsNotPositive,
    InternalInvProviderNotFound,
    MinUserPriceViolation,
    ProviderAutoDisabled,
    ProviderReEnabled,
    SymbolInProviderAutoDisabled,
    NegativeProfitQuote,
}
