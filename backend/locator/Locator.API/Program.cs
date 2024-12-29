﻿global using System;
global using System.Collections.Generic;
global using System.Linq;
using Locator.API.Data.Repositories;
using Locator.API.Data.Repositories.Interfaces;
using Locator.API.HostedServices;
using Locator.API.Models.Options;
using Locator.API.Services;
using Locator.API.Services.Interfaces;
using Locator.API.Storages;
using Locator.API.Storages.Interfaces;
using Locator.API.Utility;
using System.Globalization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using LinqToDB.AspNet;
using Locator.API.Data;
using LinqToDB;
using LinqToDB.AspNet.Logging;
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

TimeSpan GetConfigTimeSpan(string parameterName, TimeSpan defaultValue)
{
    var configString = Environment.GetEnvironmentVariable(parameterName);
    return configString == null ? defaultValue : TimeSpan.Parse(configString);
}

void AddServices(IServiceCollection services)
{
    services.AddSingleton<ILocatorService, LocatorService>();
    services.AddSingleton<IEventSender, KafkaEventSender>();
    services.AddSingleton<IInventoryService, InventoryService>();
    services.AddSingleton<ISettingService, SettingService>();
    services.AddSingleton<IPriceCalculationService, PriceCalculationService>();
    services.AddSingleton<IPriceCalculator, PriceCalculator>();

    services.AddSingleton<IPriceCalculationInfoBuilder, PriceCalculationInfoBuilder>();
    services.AddSingleton<ILocatorErrorReporter, LocatorErrorReporter>();

    services.AddSingleton<INotificationEventSender, NotificationEventSender>();
    services.AddSingleton<INotificationService, NotificationService>();
    services.AddSingleton<ITimeService, TimeService>();
    services.AddSingleton<IDataCleaner, DataCleaner>();

    services.AddSingleton<IProviderSettingRepository, ProviderSettingRepository>();

    services.AddSingleton<IProviderSettingStorage, ProviderSettingStorage>();

    services.AddSingleton<IAutoDisableProvidersService, AutoDisableProvidersService>();
}

void AddStorages(IServiceCollection services)
{
    services.AddSingleton<IQuoteStorage, QuoteStorage>();
    services.AddSingleton<IInventoryStorage, InventoryStorage>();
}

void AddConfiguration(IServiceCollection services)
{
    services.Configure<KafkaOptions>(_ =>
    {
        _.Servers = GetRequiredConfigString("KAFKA__BOOTSTRAP_SERVERS");
        _.GroupId = GetRequiredConfigString("KAFKA__GROUP_ID");
        _.QuoteRequestTopic =
            GetRequiredConfigString("KAFKA__QUOTE_REQUEST_TOPIC");
        _.QuoteResponseTopic =
            GetRequiredConfigString("KAFKA__QUOTE_RESPONSE_TOPIC");
        _.NotificationTopic =
            GetRequiredConfigString("KAFKA__NOTIFICATION_TOPIC");
        _.AddInternalInventoryItemTopic =
            GetRequiredConfigString("KAFKA__ADD_INVENTORY_ITEM_TOPIC");
        _.InvalidateCacheCommandTopic =
            GetRequiredConfigString("KAFKA__INVALIDATE_CACHE_COMMAND_TOPIC");
    });

    services.Configure<QuoteTimeoutOptions>(o =>
    {
        o.RemoveHistoryTimeout = GetConfigTimeSpan("LOCATOR__QUOTE_REMOVE_TIMEOUT", TimeSpan.FromMinutes(10));
        o.MaxProviderQuoteWait = GetConfigTimeSpan("LOCATOR__MAX_PROVIDER_QUOTE_WAIT", TimeSpan.FromSeconds(5));
        o.MaxProviderBuyWait = GetConfigTimeSpan("LOCATOR__MAX_PROVIDER_BUY_WAIT", TimeSpan.FromSeconds(30));
        o.MaxQuoteAcceptWait = GetConfigTimeSpan("LOCATOR__MAX_ACCEPT_WAIT", TimeSpan.FromSeconds(30));
    });

    services.Configure<AppOptions>(o =>
    {
        o.DayDataCleanupTimeUtc =
            TimeOnly.Parse(GetRequiredConfigString("DAY_DATA_CLEANUP_TIME_UTC"));
        o.NotificationSenderInterval =
            TimeSpan.Parse(GetRequiredConfigString("NotificationSenderInterval"));
    });

    services.Configure<AutoDisableOptions>(o =>
    {
        o.MinFailed = int.Parse(GetRequiredConfigString("AUTO_DISABLE_PROVIDER__MIN_FAILED"));
        o.SlidingWindow = TimeSpan.Parse(GetRequiredConfigString("AUTO_DISABLE_PROVIDER__SLIDING_WINDOW"));
        o.PercentOfFailed = double.Parse(GetRequiredConfigString("AUTO_DISABLE_PROVIDER__PERCENT_OF_FAILED"));
        o.TakeQuoteSuccessIntoAccount = bool.Parse(
            GetRequiredConfigString("AUTO_DISABLE_PROVIDER__TAKE_QUOTE_SUCCESS_INTO_ACCOUNT"));
    });

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
            .UseDefaultLogging(provider), ServiceLifetime.Singleton);
}

void ConfigureServices(IServiceCollection services)
{
    AddConfiguration(services);

    AddStorages(services);

    AddServices(services);

    services.AddHostedService<LocatorQuoteResponseKafkaListener>();
    services.AddHostedService<SyncCommandListener>();
    services.AddHostedService<ProviderQuoteResponseKafkaListener>();
    services.AddHostedService<ProviderBuyOrderResponseKafkaListener>();
    services.AddHostedService<QuoteRequestKafkaListener>();
    services.AddHostedService<QuoteTimeoutProcessor>();
    services.AddHostedService<DayDataCleaner>();
    services.AddHostedService<NotificationSender>();

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