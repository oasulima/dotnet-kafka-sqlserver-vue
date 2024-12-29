using LinqToDB.Mapping;
using Shared;

namespace InternalInventory.API.Data.Entities;

[Table("InternalInventoryItems", Schema = "dbo")]
public class InternalInventoryItemDb
{
	[Column("Id", CanBeNull = false, IsPrimaryKey = true, PrimaryKeyOrder = 0)] public string Id { get; set; } = null!; // varchar(100)
	[Column("Version", IsPrimaryKey = true, PrimaryKeyOrder = 1)] public int Version { get; set; } // int
	[Column("Symbol", CanBeNull = false)] public string Symbol { get; set; } = null!; // varchar(10)
	[Column("Price")] public decimal Price { get; set; } // decimal(10, 5)
	[Column("Quantity")] public int Quantity { get; set; } // int
	[Column("SoldQuantity")] public int SoldQuantity { get; set; } // int
	[Column("Source", CanBeNull = false)] public string Source { get; set; } = null!; // varchar(max)
	[Column("Status", CanBeNull = false)] public string Status { get; set; } = null!; // varchar(20)
	[Column("CreatedAt")] public DateTime CreatedAt { get; set; } // datetime2(7)
	[Column("Timestamp")] public DateTime Timestamp { get; set; } = DateTime.UtcNow;// datetime2(7)
	[Column("Tag")] public string? Tag { get; set; } // varchar(max)
	[Column("CoveredInvItemId")] public string? CoveredInvItemId { get; set; } // varchar(100)
	[Column("CreatingType", CanBeNull = false)] public CreatingType CreatingType { get; set; } // varchar(40)
}
