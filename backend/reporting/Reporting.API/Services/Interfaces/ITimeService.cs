using System;

namespace Reporting.API.Services.Interfaces;

public interface ITimeService
{
    DateTime GetNextCleanupTimeInUtc(DateTime utcNow);
}