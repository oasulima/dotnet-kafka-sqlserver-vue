namespace Locator.API.Models.Options;

public class QuoteTimeoutOptions
{
    public required TimeSpan MaxProviderQuoteWait { get; set; }
    public required TimeSpan MaxProviderBuyWait { get; set; }
    public required TimeSpan MaxQuoteAcceptWait { get; set; }
    public required TimeSpan RemoveHistoryTimeout { get; set; }
}
