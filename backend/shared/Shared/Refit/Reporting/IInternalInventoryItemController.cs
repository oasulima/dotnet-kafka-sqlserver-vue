using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace Shared.Refit.Reporting;

public interface IInternalInventoryItemController
{
    [Get("/api/internal-inventory/items/history")]
    Task<IEnumerable<InternalInventoryItem>> GetInternalInventoryItemsHistory(
        int take, string symbol = null, string providerId = null,
        DateTime? beforeCreatedAt = null);


    [Get("/api/internal-inventory/items")]
    Task<IEnumerable<InternalInventoryItem>> GetInternalInventoryItems(
        DateTime? from = null, DateTime? to = null, string symbol = null,
        CreatingType? creatingType = null, 
        InternalInventoryItem.State? status = null);

}