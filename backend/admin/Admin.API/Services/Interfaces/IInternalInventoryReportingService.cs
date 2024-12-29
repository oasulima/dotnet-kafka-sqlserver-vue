using Shared;

namespace Admin.API.Services.Interfaces;

public interface IInternalInventoryReportingService
{
    Task<IEnumerable<InternalInventoryItem>> GetPreviousDaysInventoryItems(int take, string? providerId, string? symbol = null,
        DateTime? beforeDateTime = null);

    Task<IEnumerable<InternalInventoryItem>> GetInternalInventoryItems(string? symbol = null, CreatingType? creatingType = null,
        InternalInventoryItem.State? status = null);
}