namespace Locator.API.Models.Options;

public class AutoDisableOptions
{
    public required int MinFailed { get; set; } = 2;
    public required double PercentOfFailed { get; set; } = 0.1;
    public required TimeSpan SlidingWindow { get; set; } = TimeSpan.FromMinutes(10);
    public required bool TakeQuoteSuccessIntoAccount { get; set; } = false;
}
