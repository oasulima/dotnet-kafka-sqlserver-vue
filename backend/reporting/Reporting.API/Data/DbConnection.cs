using LinqToDB;
using LinqToDB.Data;
using Newtonsoft.Json;
using Reporting.API.Data.Models.DbModels;
using Shared;

namespace Reporting.API.Data;

public class DbConnection : DataConnection
{
	public DbConnection(DataOptions<DbConnection> options)
			: base(options.Options)
	{

		MappingSchema.AddScalarType(typeof(QuoteSourceInfoDb[]), DataType.VarChar);
		MappingSchema.SetConverter<string, QuoteSourceInfoDb[]>(v =>
		{
			Console.WriteLine($"string to QuoteSourceInfoDb converter: {v}");
			return JsonConvert.DeserializeObject<QuoteSourceInfoDb[]>(v) ?? Array.Empty<QuoteSourceInfoDb>();
		}, LinqToDB.Common.ConversionType.FromDatabase);
		MappingSchema.SetConverter<string, CreatingType>(v =>
		{
			Console.WriteLine($"string to CreatingType converter: {v}");
			return Enum.Parse<CreatingType>(v);
		}, LinqToDB.Common.ConversionType.FromDatabase);
	}
}