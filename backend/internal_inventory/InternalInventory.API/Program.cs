global using System;
global using System.Collections.Generic;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using InternalInventory.API.HostedServices;
using InternalInventory.API.Models.Options;
using InternalInventory.API.Services;
using InternalInventory.API.Services.Interfaces;
using InternalInventory.API.Storages;
using InternalInventory.API.Storages.Interfaces;
using InternalInventory.API.Utilities;
using System.Globalization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using FluentValidation;
using Shared.Refit;
using Refit;
using LinqToDB.AspNet;
using LinqToDB;
using LinqToDB.AspNet.Logging;
using InternalInventory.API.Data;
using System.Reflection;
using System.Diagnostics;

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
    services.Configure<SelfOptions>(ConfigureSelfOptions);
    services.Configure<KafkaOptions>(ConfigureKafkaOptions);
    services.Configure<AppOptions>(ConfigureAppOptions);

    services.AddSingleton<IInventoryStorage, InventoryStorage>();
    services.AddSingleton<IInventoryService, InventoryService>();
    services.AddSingleton<IEventSender, KafkaEventSender>();
    services.AddSingleton<ITimeService, TimeService>();
    services.AddSingleton<IDataCleaner, DataCleaner>();
    services.AddSingleton<IProviderSettingService, ProviderSettingService>();

    services.AddHostedService<QuoteRequestKafkaListener>();
    services.AddHostedService<BuyRequestKafkaListener>();
    services.AddHostedService<SelfRegHostedServices>();
    services.AddHostedService<AddInventoryItemKafkaListener>();
    services.AddHostedService<DayDataCleaner>();

    AddValidation(services);

    var LocatorBaseUrl = GetRequiredConfigString("LOCATOR__BASE_URL");

    services
        .AddRefitClient<ILocatorApi>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(GetRequiredConfigString("LOCATOR__BASE_URL")));

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

void ConfigureSelfOptions(SelfOptions o)
{
    o.Name = GetRequiredConfigString("SELF__NAME");
    o.Id = GetRequiredConfigString("SELF__ID");
}

void ConfigureKafkaOptions(KafkaOptions o)
{
    o.Servers = GetRequiredConfigString("KAFKA__BOOTSTRAP_SERVERS");
    o.OrderReportTopic = GetRequiredConfigString("KAFKA__ORDER_REPORT_TOPIC");
    o.QuoteResponseTopic = GetRequiredConfigString("KAFKA__QUOTE_RESPONSE_TOPIC");
    o.BuyRequestTopic = GetRequiredConfigString("KAFKA__BUY_REQUEST_TOPIC");
    o.QuoteRequestTopic = GetRequiredConfigString("KAFKA__QUOTE_REQUEST_TOPIC");
    o.AddInventoryItemTopic = GetRequiredConfigString("KAFKA__ADD_INVENTORY_ITEM_TOPIC");
    o.InternalInventoryItemReportingTopic =
        GetRequiredConfigString("KAFKA__INTERNAL_INVENTORY_ITEM_REPORTING_TOPIC");
    o.GroupId = GetRequiredConfigString("KAFKA__GROUP_ID");
}

void ConfigureAppOptions(AppOptions o)
{
    o.DayDataCleanupTimeUtc = TimeOnly.Parse(GetRequiredConfigString("DAY_DATA_CLEANUP_TIME_UTC"));
}

var builder = WebApplication.CreateBuilder(args);

TracingConfiguration.ConfigureOpenTelemetry(Assembly.GetExecutingAssembly());

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

