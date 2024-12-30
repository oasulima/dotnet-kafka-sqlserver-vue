using LinqToDB;
using LinqToDB.Data;
using Locator.API.Data.Entities;

namespace Locator.API.Data;

public class DbConnection : DataConnection
{
    public DbConnection(DataOptions<DbConnection> options)
        : base(options.Options)
    {
    }

    public ITable<AccountInventoryItemDb> AccountInventoryItems => this.GetTable<AccountInventoryItemDb>();
    public ITable<ProviderSettingDb> ProviderSettings => this.GetTable<ProviderSettingDb>();
}