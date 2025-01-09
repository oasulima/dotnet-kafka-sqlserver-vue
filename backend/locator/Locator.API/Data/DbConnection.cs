using LinqToDB;
using Locator.API.Data.Entities;
using Shared;

namespace Locator.API.Data;

public class DbConnection : BaseDbConnection<DbConnection>
{
    public DbConnection(DataOptions<DbConnection> options)
        : base(options) { }

    public ITable<AccountInventoryItemDb> AccountInventoryItems =>
        this.GetTable<AccountInventoryItemDb>();
    public ITable<ProviderSettingDb> ProviderSettings => this.GetTable<ProviderSettingDb>();
}
