using LinqToDB.Mapping;

namespace Locator.API.Data.Entities;

[Table("AccountInventoryItems", Schema = "dbo")]
public class AccountInventoryItemDb
{
    [Column("Id", CanBeNull = false, IsPrimaryKey = true, PrimaryKeyOrder = 0)]
    public required string Id { get; set; } // varchar(100)

    [Column("Version", IsPrimaryKey = true, PrimaryKeyOrder = 1)]
    public int Version { get; set; } // int

    [Column("AccountId", CanBeNull = false)]
    public required string AccountId { get; set; } // varchar(200)

    [Column("Symbol", CanBeNull = false)]
    public required string Symbol { get; set; } // varchar(20)

    [Column("LocatedQuantity")]
    public int LocatedQuantity { get; set; } // int

    [Column("AvailableQuantity")]
    public int AvailableQuantity { get; set; } // int

    [Column("Timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow; // datetime2(7)
}
