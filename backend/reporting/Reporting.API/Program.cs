using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Reporting.API.Data.Repositories;
using Reporting.API.Data.Repositories.Interfaces;
using Reporting.API.HostedServices;
using Reporting.API.Models.Options;
using Reporting.API.Services;
using Reporting.API.Services.Interfaces;
using Reporting.API.Utility;
using System.Globalization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using LinqToDB.AspNet;
using Reporting.API.Data;
using LinqToDB;
using LinqToDB.AspNet.Logging;
using System.Diagnostics;
using System.Reflection;

string GetRequiredConfigString(string parameterName)
{
    var configString = Environment.GetEnvironmentVariable(parameterName);
    if (configString == null)
    {
        var message = $"Configuration Exception: {parameterName} is not configured";
        Console.WriteLine(message);
        throw new Exception(message);
    }

    return configString;
}



void ConfigureServices(IServiceCollection services)
{

    AddOptions(services);
    AddServices(services);
    AddValidation(services);

    var connectionString = GetRequiredConfigString("CONNECTION_STRING");

    services.AddLinqToDBContext<DbConnection>((provider, options)
        => options
            //will configure the AppDataConnection to use
            //SqlServer with the provided connection string
            //there are methods for each supported database
            .UseSqlServer(connectionString)
            //default logging will log everything using
            //an ILoggerFactory configured in the provider
            .UseTraceLevel(System.Diagnostics.TraceLevel.Verbose)
            .UseTraceMapperExpression(true)
            .UseTracing(TraceLevel.Verbose, ti =>
            {
                using var activity = TracingConfiguration.StartActivity("linq2db trace");
                activity.SetTag("Command", ti.Command);
                activity.SetTag("CommandText", ti.CommandText);
                if (ti.Exception != null)
                {
                    activity.AddException(ti.Exception);
                }
                activity.SetTag("MapperExpression", ti.MapperExpression);
                activity.SetTag("SqlText", ti.SqlText);
                activity.SetTag("TraceInfoStep", ti.TraceInfoStep);
            })
            .UseDefaultLogging(provider), ServiceLifetime.Scoped);


}



void AddValidation(IServiceCollection services)
{

    services.AddValidatorsFromAssemblyContaining<Program>();
    services.AddFluentValidationAutoValidation();

    services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
}

void AddServices(IServiceCollection services)
{
    services.AddHostedService<LocatorQuoteResponseKafkaListener>();
    services.AddHostedService<InternalInventoryItemKafkaListener>();
    services.AddHostedService<QuoteRequestKafkaListener>();

    services.AddSingleton<ILocatorQuoteResponseRepository, LocatorQuoteResponseRepository>();
    services.AddSingleton<IQuoteDetailsRepository, QuoteDetailsRepository>();
    services.AddSingleton<IInternalInventoryItemRepository, InternalInventoryItemRepository>();
    services.AddSingleton<IQuoteRequestRepository, QuoteRequestRepository>();
    services.AddSingleton<ILocatesReportDataService, LocatesReportDataService>();
    services.AddSingleton<ITimeService, TimeService>();
}

void AddOptions(IServiceCollection services)
{
    services.Configure<KafkaOptions>(_ =>
    {
        _.Servers = GetRequiredConfigString("KAFKA__BOOTSTRAP_SERVERS");
        _.GroupId = GetRequiredConfigString("KAFKA__GROUP_ID");
        _.QuoteRequestTopic =
            GetRequiredConfigString("KAFKA__QUOTE_REQUEST_TOPIC");
        _.QuoteResponseTopic =
            GetRequiredConfigString("KAFKA__QUOTE_RESPONSE_TOPIC");
        _.InternalInventoryItemReportingTopic =
            GetRequiredConfigString("KAFKA__INTERNAL_INVENTORY_ITEM_REPORTING_TOPIC");
    });

    services.Configure<AppOptions>(o =>
    {
        o.DayDataCleanupTimeUtc =
            TimeOnly.Parse(GetRequiredConfigString("DAY_DATA_CLEANUP_TIME_UTC"));
    });
}

var builder = WebApplication.CreateBuilder(args);

TracingConfiguration.ConfigureOpenTelemetry(Assembly.GetExecutingAssembly());

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHealthChecks();

// Add services to the container.

builder.Services.AddControllers(options => { options.Filters.Add(typeof(ValidateModelAttribute)); })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.Converters.Add(new IsoDateTimeConverter
        {
            DateTimeStyles = DateTimeStyles.AdjustToUniversal
        });
    });

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    DateTimeZoneHandling = DateTimeZoneHandling.Utc
};

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SupportNonNullableReferenceTypes();
});
builder.Services.AddSwaggerGenNewtonsoftSupport();

ConfigureServices(builder.Services);

builder.Services.AddResponseCompression();

var app = builder.Build();

app.MapHealthChecks("/hc");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseResponseCompression();


app.MapControllers();



app.Run();
