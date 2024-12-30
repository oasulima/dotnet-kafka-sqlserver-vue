using LinqToDB.Mapping;

namespace Locator.API.Data.Entities;

[Table("ProviderSetting", Schema = "dbo")]
public class ProviderSettingDb
{
	[Column("ProviderId", CanBeNull = false, IsPrimaryKey = true)] public required string ProviderId { get; set; } // varchar(100)
	[Column("Name", CanBeNull = false)] public required string Name { get; set; } // varchar(100)
	[Column("Discount")] public decimal Discount { get; set; } // decimal(10, 5)
	[Column("Active")] public bool Active { get; set; } // bit
	[Column("BuyRequestTopic")] public string? BuyRequestTopic { get; set; } // varchar(max)
	[Column("BuyResponseTopic")] public string? BuyResponseTopic { get; set; } // varchar(max)
	[Column("QuoteRequestTopic")] public string? QuoteRequestTopic { get; set; } // varchar(max)
	[Column("QuoteResponseTopic")] public string? QuoteResponseTopic { get; set; } // varchar(max) 
}
