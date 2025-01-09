namespace Admin.API.Services.Interfaces;

public interface ITimeService
{
    DateTime GetPreviousCleanupTimeInUtc(DateTime now);
    DateTime GetNextCleanupTimeInUtc(DateTime now);
    TimeSpan GetNextCleanupDelay(DateTime now);
}
