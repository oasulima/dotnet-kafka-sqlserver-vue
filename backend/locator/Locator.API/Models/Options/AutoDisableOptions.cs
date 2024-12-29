namespace Locator.API.Models.Options;

public class AutoDisableOptions
{
    public int MinFailed { get; set; } = 2;
    public double PercentOfFailed { get; set; } = 0.1;
    public TimeSpan SlidingWindow { get; set; } = TimeSpan.FromMinutes(10);
    public bool TakeQuoteSuccessIntoAccount { get; set; } = false;
}