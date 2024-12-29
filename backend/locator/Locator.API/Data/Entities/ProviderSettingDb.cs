using LinqToDB.Mapping;

namespace Locator.API.Data.Entities;

[Table("ProviderSetting", Schema = "dbo")]
public class ProviderSettingDb
{
	[Column("ProviderId", CanBeNull = false, IsPrimaryKey = true)] public string ProviderId { get; set; } = null!; // varchar(100)
	[Column("Name", CanBeNull = false)] public string Name { get; set; } = null!; // varchar(100)
	[Column("Discount")] public decimal Discount { get; set; } // decimal(10, 5)
	[Column("Active")] public bool Active { get; set; } // bit
	[Column("BuyRequestTopic")] public string? BuyRequestTopic { get; set; } // varchar(max)
	[Column("BuyResponseTopic")] public string? BuyResponseTopic { get; set; } // varchar(max)
	[Column("QuoteRequestTopic")] public string? QuoteRequestTopic { get; set; } // varchar(max)
	[Column("QuoteResponseTopic")] public string? QuoteResponseTopic { get; set; } // varchar(max) 
}
