using Admin.API.Services.Interfaces;
using Shared;
using Shared.Refit;

namespace Admin.API.Services;

public class InternalInventoryReportingService : IInternalInventoryReportingService
{
    private readonly ITimeService _timeService;
    private readonly IReportingApi _reportingApi;

    public InternalInventoryReportingService(ITimeService timeService,
        IReportingApi reportingApi)
    {
        _timeService = timeService;
        _reportingApi = reportingApi;
    }

    public Task<IEnumerable<InternalInventoryItem>> GetPreviousDaysInventoryItems(int take, string? providerId,
        string? symbol = null, DateTime? currentDateTime = null)
    {
        DateTime? previousCleanup = null;
        if (currentDateTime != null)
        {
            previousCleanup = _timeService.GetPreviousCleanupTimeInUtc(currentDateTime.Value);
        }

        return _reportingApi.GetInternalInventoryItemsHistory(take, symbol, providerId, previousCleanup);
    }

    public Task<IEnumerable<InternalInventoryItem>> GetInternalInventoryItems(string? symbol = null, CreatingType? creatingType = null,
        InternalInventoryItem.State? status = null)
    {
        var from = _timeService.GetPreviousCleanupTimeInUtc(DateTime.UtcNow);

        return _reportingApi.GetInternalInventoryItems(from, null, symbol, creatingType, status);
    }
}