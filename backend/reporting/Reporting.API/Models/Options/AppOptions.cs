using System;

namespace Reporting.API.Models.Options;

public class AppOptions
{
    public required TimeOnly DayDataCleanupTimeUtc { get; set; }
}
