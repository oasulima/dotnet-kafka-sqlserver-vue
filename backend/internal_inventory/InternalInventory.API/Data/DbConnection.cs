using LinqToDB;
using LinqToDB.Data;
using InternalInventory.API.Data.Entities;

namespace InternalInventory.API.Data;

public class DbConnection : DataConnection
{
    public DbConnection(DataOptions<DbConnection> options)
        : base(options.Options)
    { }

    public ITable<InternalInventoryItemDb> InternalInventoryItems => this.GetTable<InternalInventoryItemDb>();
}