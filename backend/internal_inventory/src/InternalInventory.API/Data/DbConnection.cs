using InternalInventory.API.Data.Entities;
using LinqToDB;
using Shared;

namespace InternalInventory.API.Data;

public class DbConnection : BaseDbConnection<DbConnection>
{
    public DbConnection(DataOptions<DbConnection> options)
        : base(options) { }

    public ITable<InternalInventoryItemDb> InternalInventoryItems =>
        this.GetTable<InternalInventoryItemDb>();
}
