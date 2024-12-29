using System;
using Microsoft.Extensions.Options;
using Admin.API.Options;
using Admin.API.Services.Interfaces;

namespace Admin.API.Services;

public class TimeService : ITimeService
{
    private readonly IOptions<AppOptions> _appOptions;

    public TimeService(IOptions<AppOptions> appOptions)
    {
        _appOptions = appOptions;
    }

    public DateTime GetPreviousCleanupTimeInUtc(DateTime now)
    {
        var utcNow = now.ToUniversalTime();

        var previousCleanup = utcNow.Date.Add(_appOptions.Value.DayDataCleanupTimeUtc.ToTimeSpan());
        if (previousCleanup > now)
        {
            previousCleanup = previousCleanup.AddDays(-1);
        }

        return previousCleanup;
    }

    public DateTime GetNextCleanupTimeInUtc(DateTime now)
    {
        var utcNow = now.ToUniversalTime();

        var nextCleanup = utcNow.Date.Add(_appOptions.Value.DayDataCleanupTimeUtc.ToTimeSpan());
        if (nextCleanup < now)
        {
            nextCleanup = nextCleanup.AddDays(1);
        }

        return nextCleanup;
    }

    public TimeSpan GetNextCleanupDelay(DateTime now)
    {
        var utcNow = now.ToUniversalTime();
        var nextCleanup = GetNextCleanupTimeInUtc(utcNow);
        return nextCleanup - utcNow;
    }
}