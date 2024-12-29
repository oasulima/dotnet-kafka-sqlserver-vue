using LinqToDB.Mapping;
using Shared;

namespace Locator.API.Data.Entities;

[Table("AccountInventoryItems", Schema = "dbo")]
public class AccountInventoryItemDb
{
	[Column("Id", CanBeNull = false, IsPrimaryKey = true, PrimaryKeyOrder = 0)] public string Id { get; set; } = null!; // varchar(100)
	[Column("Version", IsPrimaryKey = true, PrimaryKeyOrder = 1)] public int Version { get; set; } // int
	[Column("AccountId", CanBeNull = false)] public string AccountId { get; set; } = null!; // varchar(200)
	[Column("Symbol", CanBeNull = false)] public string Symbol { get; set; } = null!; // varchar(20)
	[Column("LocatedQuantity")] public int LocatedQuantity { get; set; } // int
	[Column("AvailableQuantity")] public int AvailableQuantity { get; set; } // int
	[Column("OriginalSource", CanBeNull = false)] public string OriginalSource { get; set; } = null!; // varchar(200)
	[Column("OriginalPrice")] public decimal OriginalPrice { get; set; } // decimal(10, 5)
	[Column("Timestamp")] public DateTime Timestamp { get; set; } = DateTime.UtcNow; // datetime2(7)
}
