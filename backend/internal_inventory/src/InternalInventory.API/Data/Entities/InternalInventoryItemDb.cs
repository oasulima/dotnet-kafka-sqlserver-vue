using LinqToDB.Mapping;
using Shared;
using static Shared.InternalInventoryItem;

namespace InternalInventory.API.Data.Entities;

[Table("InternalInventoryItems", Schema = "dbo")]
public class InternalInventoryItemDb
{
    [Column("Id", CanBeNull = false, IsPrimaryKey = true, PrimaryKeyOrder = 0)]
    public required string Id { get; set; }

    [Column("Version", IsPrimaryKey = true, PrimaryKeyOrder = 1)]
    public int Version { get; set; } // int

    [Column("Symbol", CanBeNull = false)]
    public required string Symbol { get; set; }

    [Column("Price")]
    public decimal Price { get; set; } // decimal(10, 5)

    [Column("Quantity")]
    public int Quantity { get; set; } // int

    [Column("SoldQuantity")]
    public int SoldQuantity { get; set; } // int

    [Column("Source", CanBeNull = false)]
    public required string Source { get; set; }

    [Column("Status", CanBeNull = false)]
    public required State Status { get; set; }

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } // datetime2(7)

    [Column("Timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow; // datetime2(7)

    [Column("Tag")]
    public string? Tag { get; set; } // varchar(max)

    [Column("CoveredInvItemId")]
    public string? CoveredInvItemId { get; set; } // varchar(100)

    [Column("CreatingType", CanBeNull = false, DataType = LinqToDB.DataType.VarChar)]
    public required CreatingType CreatingType { get; set; }
}
