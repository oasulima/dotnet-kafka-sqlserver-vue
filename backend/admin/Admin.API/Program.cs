using System.Globalization;
using System.Reflection;
using Admin.API;
using Admin.API.HostedServices;
using Admin.API.Options;
using Admin.API.Services;
using Admin.API.Services.Interfaces;
using Admin.API.Utility;
using Lib.AspNetCore.ServerSentEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Shared;
using Shared.Refit;

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
    services.AddTransient<INotificationsServiceBase, NotificationsServiceBase>();

    services.AddSingleton<IProviderSettingCache, ProviderSettingCache>();
    services.AddSingleton<ILocatesCache, LocatesCache>();
    services.AddSingleton<ILocateRequestsCache, LocateRequestsCache>();
    services.AddSingleton<IMessageHandler, MessageHandler>();
    services.AddSingleton<ITimeService, TimeService>();
    services.AddSingleton<IInternalInventoryReportingService, InternalInventoryReportingService>();
    services.AddSingleton<IEventSender, KafkaEventSender>();

    services.AddHostedService<LocatorQuoteResponseKafkaListener>();
    services.AddHostedService<NotificationKafkaListener>();
    services.AddHostedService<DayDataCleaner>();
    services.AddHostedService<SyncCommandListener>();
    services.AddHostedService<InternalInventoryItemChangedListener>();

    services.Configure<KafkaOptions>(_ =>
    {
        _.Servers = GetRequiredConfigString(Constants.ENV.KAFKA__BOOTSTRAP_SERVERS);
        _.GroupId = GetRequiredConfigString("KAFKA__GROUP_ID");
        _.LocatorQuoteResponseTopic = GetRequiredConfigString(
            Constants.ENV.KAFKA__LOCATOR_QUOTE_RESPONSE_TOPIC
        );
        _.LocatorQuoteRequestTopic = GetRequiredConfigString(
            Constants.ENV.KAFKA__LOCATOR_QUOTE_REQUEST_TOPIC
        );
        _.NotificationTopic = GetRequiredConfigString(Constants.ENV.KAFKA__NOTIFICATION_TOPIC);
        _.InvalidateCacheCommandTopic = GetRequiredConfigString(
            Constants.ENV.KAFKA__INVALIDATE_CACHE_COMMAND_TOPIC
        );
        _.InternalInventoryItemChangeTopic = GetRequiredConfigString(
            "KAFKA__INTERNAL_INVENTORY_ITEM_REPORTING_TOPIC"
        );
    });

    services.Configure<AppOptions>(_ =>
    {
        _.DayDataCleanupTimeUtc = TimeOnly.Parse(
            GetRequiredConfigString(Constants.ENV.DATA_CLEANER_RUN_TIME_UTC)
        );
    });

    var httpResilienceOptions = new HttpResilienceOptions
    {
        MaxRetryAttempts = int.Parse(GetRequiredConfigString("HTTP_RESILIENCE_MAX_RETRY_ATTEMPTS")),
        RetryDelay = TimeSpan.Parse(GetRequiredConfigString("HTTP_RESILIENCE_RETRY_DELAY")),
        AttemptTimeout = TimeSpan.Parse(GetRequiredConfigString("HTTP_RESILIENCE_ATTEMPT_TIMEOUT")),
        TotalRequestTimeout = TimeSpan.Parse(
            GetRequiredConfigString("HTTP_RESILIENCE_TOTAL_REQUEST_TIMEOUT")
        ),
        CircuitBreakerSamplingDuration = TimeSpan.Parse(
            GetRequiredConfigString("HTTP_RESILIENCE_CIRCUIT_BREAKER_SAMPLING_DURATION")
        ),
        HttpClientTimeout = TimeSpan.Parse(
            GetRequiredConfigString("HTTP_RESILIENCE_HTTP_CLIENT_TIMEOUT")
        ),
    };

    var InternalInventoryBaseUrl = GetRequiredConfigString(
        Constants.ENV.INTERNAL_INVENTORY_BASE_URL
    );
    var LocatorBaseUrl = GetRequiredConfigString(Constants.ENV.LOCATOR_BASE_URL);
    var ReportingBaseUrl = GetRequiredConfigString(Constants.ENV.REPORTING_BASE_URL);

    services.RegisterExternalAPI<IInternalInventoryApi>(
        InternalInventoryBaseUrl,
        httpResilienceOptions
    );
    services.RegisterExternalAPI<ILocatorApi>(LocatorBaseUrl, httpResilienceOptions);
    services.RegisterExternalAPI<IReportingApi>(ReportingBaseUrl, httpResilienceOptions);
}

var builder = WebApplication.CreateBuilder(args);

TracingConfiguration.ConfigureOpenTelemetry(Assembly.GetExecutingAssembly());

string corsSites = GetRequiredConfigString("CORSSites");
string[] sites = corsSites.Split(',');

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(sites).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

builder.Services.AddServerSentEvents<
    INotificationsServerSentEventsService,
    NotificationsServerSentEventsService
>(options =>
{
    options.ReconnectInterval = 5000;
});

// Add services to the container.

JsonConvert.DefaultSettings = () => Converter.Settings;

builder
    .Services.AddControllers(options =>
    {
        options.Filters.Add(typeof(ValidateModelAttribute));
    })
    .AddNewtonsoftJson(o =>
    {
        o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseResponseCompression();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.MapServerSentEvents<NotificationsServerSentEventsService>("/sse");

app.Run();
