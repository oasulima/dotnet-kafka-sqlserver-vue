namespace InternalInventory.API.Services.Interfaces;

public interface ITimeService
{
    DateTime GetNextCleanupTimeInUtc(DateTime now);
    TimeSpan GetNextCleanupDelay(DateTime now);
}
