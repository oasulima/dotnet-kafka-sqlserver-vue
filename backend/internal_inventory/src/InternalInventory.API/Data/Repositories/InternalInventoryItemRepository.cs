using InternalInventory.API.Data.Entities;
using LinqToDB;
using LinqToDB.Data;

namespace InternalInventory.API.Data.Repositories;

public class InternalInventoryItemRepository : IInternalInventoryItemRepository
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public InternalInventoryItemRepository(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public InternalInventoryItemDb[] Get(DateTime? afterDateTime = null)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        return linq2db
            .InternalInventoryItems.Where(x => afterDateTime == null || x.Timestamp > afterDateTime)
            .ToArray();
    }

    public void Add(InternalInventoryItemDb inventoryItem)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        linq2db.Insert(inventoryItem);
    }

    public void Add(IList<InternalInventoryItemDb> inventoryItems)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        linq2db.BulkCopy(inventoryItems);
    }

    public void DeleteAll()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        linq2db.InternalInventoryItems.Delete();
    }
}
