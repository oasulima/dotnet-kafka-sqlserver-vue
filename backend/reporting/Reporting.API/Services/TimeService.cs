using System;
using Microsoft.Extensions.Options;
using Reporting.API.Models.Options;
using Reporting.API.Services.Interfaces;

namespace Reporting.API.Services;

public class TimeService : ITimeService
{
    private readonly IOptions<AppOptions> _appOptions;

    public TimeService(IOptions<AppOptions> appOptions)
    {
        _appOptions = appOptions;
    }

    public DateTime GetNextCleanupTimeInUtc(DateTime utcNow)
    {
        var nextCleanup = utcNow.Date.Add(_appOptions.Value.DayDataCleanupTimeUtc.ToTimeSpan());
        if (nextCleanup < utcNow)
        {
            nextCleanup = nextCleanup.AddDays(1);
        }

        return nextCleanup;
    }
}