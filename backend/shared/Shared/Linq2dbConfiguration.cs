using System.Diagnostics;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Shared;

public abstract class BaseDbConnection<T> : DataConnection
    where T : LinqToDB.IDataContext
{
    public BaseDbConnection(DataOptions<T> options)
        : base(options.Options)
    {
        RegisterMapping_CreatingType_to_VarChar();
        RegisterMapping_State_to_VarChar();

        //  MappingSchema.AddScalarType(typeof(QuoteSourceInfoDb[]), DataType.VarChar);
        // MappingSchema.SetConverter<string, QuoteSourceInfoDb[]>(
        //     v =>
        //     {
        //         Console.WriteLine($"string to QuoteSourceInfoDb converter: {v}");
        //         return JsonConvert.DeserializeObject<QuoteSourceInfoDb[]>(v)
        //             ?? Array.Empty<QuoteSourceInfoDb>();
        //     },
        //     LinqToDB.Common.ConversionType.FromDatabase
        // );
        // MappingSchema.SetConverter<string, CreatingType>(
        //     v =>
        //     {
        //         Console.WriteLine($"string to CreatingType converter: {v}");
        //         return Enum.Parse<CreatingType>(v);
        //     },
        //     LinqToDB.Common.ConversionType.FromDatabase
        // );
    }

    private void RegisterMapping_State_to_VarChar()
    {
        MappingSchema.AddScalarType(typeof(InternalInventoryItem.State), DataType.VarChar);

        MappingSchema.SetConverter<DataParameter, InternalInventoryItem.State>(
            v => Enum.Parse<InternalInventoryItem.State>(v.Value?.ToString() ?? ""),
            LinqToDB.Common.ConversionType.FromDatabase
        );
        MappingSchema.SetConverter<InternalInventoryItem.State, DataParameter>(
            v => DataParameter.VarChar(null, v.ToString()),
            LinqToDB.Common.ConversionType.ToDatabase
        );
    }

    private void RegisterMapping_CreatingType_to_VarChar()
    {
        MappingSchema.AddScalarType(typeof(CreatingType), DataType.VarChar);

        MappingSchema.SetConverter<CreatingType, DataParameter>(
            v => DataParameter.VarChar(null, v.ToString()),
            LinqToDB.Common.ConversionType.ToDatabase
        );
        MappingSchema.SetConverter<DataParameter, CreatingType>(
            v => Enum.Parse<CreatingType>(v.Value?.ToString() ?? ""),
            LinqToDB.Common.ConversionType.FromDatabase
        );
    }
}

public static class Linq2dbConfiguration
{
    public static void RegisterLinq2db<TDbConnection>(
        IServiceCollection services,
        string connectionString
    )
        where TDbConnection : BaseDbConnection<TDbConnection>
    {
        services.AddLinqToDBContext<TDbConnection>(
            (provider, options) =>
                options
                    //will configure the AppDataConnection to use
                    //SqlServer with the provided connection string
                    //there are methods for each supported database
                    .UseSqlServer(connectionString)
                    //default logging will log everything using
                    //an ILoggerFactory configured in the provider
                    .UseTraceLevel(System.Diagnostics.TraceLevel.Verbose)
                    .UseTraceMapperExpression(true)
                    .UseTracing(
                        TraceLevel.Verbose,
                        ti =>
                        {
                            using var activity = TracingConfiguration.StartActivity(
                                "linq2db trace"
                            );
                            activity.SetTag("Command", ti.Command);
                            activity.SetTag("CommandText", ti.CommandText);
                            if (ti.Exception != null)
                            {
                                activity.AddException(ti.Exception);
                            }
                            activity.SetTag("MapperExpression", ti.MapperExpression);
                            activity.SetTag("SqlText", ti.SqlText);
                            activity.SetTag("TraceInfoStep", ti.TraceInfoStep);
                        }
                    )
                    .UseDefaultLogging(provider),
            ServiceLifetime.Scoped
        );
    }
}
