namespace Locator.API.Models.Options;

public class QuoteTimeoutOptions
{
    public TimeSpan MaxProviderQuoteWait { get; set; }
    public TimeSpan MaxProviderBuyWait { get; set; }
    public TimeSpan MaxQuoteAcceptWait { get; set; }
    public TimeSpan RemoveHistoryTimeout { get; set; }
}