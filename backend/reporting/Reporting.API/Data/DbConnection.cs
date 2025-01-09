using LinqToDB;
using Shared;

namespace Reporting.API.Data;

public class DbConnection : BaseDbConnection<DbConnection>
{
    public DbConnection(DataOptions<DbConnection> options)
        : base(options) { }
}
