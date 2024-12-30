// See https://aka.ms/new-console-template for more information

using TradeZero.Locator.Emulator;
using TradeZero.Locator.Emulator.Kafka;
using TradeZero.Locator.Emulator.Options;
using TradeZero.Locator.Emulator.Services;

static string GetRequiredConfigString(string parameterName)
{
    var configString = Environment.GetEnvironmentVariable(parameterName);
    if (configString == null)
    {
        var message = $"Configuration Exception: {parameterName} is not configured";
        SharedData.Log(message);
        throw new Exception(message);
    }

    return configString;
}

static int GetRequiredConfigInt(string parameterName)
{
    var configString = Environment.GetEnvironmentVariable(parameterName);

    if (configString == null || !int.TryParse(configString, out var result))
    {
        var message = $"Configuration Exception: {parameterName} is not configured";
        SharedData.Log(message);
        throw new Exception(message);
    }

    return result;
}

var cts = new CancellationTokenSource();
var allowNewQuotes = new CancellationTokenSource();
var backgroundTasks = new List<Task>();

Console.CancelKeyPress += (s, e) =>
{
    SharedData.Log("Canceling...");
    cts.Cancel();
    e.Cancel = true;
    StatisticsPrinter.Print();
};

var emulatorOptions = new EmulatorOptions
{
    EmulationLength = TimeSpan.Parse(GetRequiredConfigString("EMULATION_LENGTH")),
    NumberOfSymbols = GetRequiredConfigInt("NUMBER_OF_SYMBOLS"),
    NumberOfSenders = GetRequiredConfigInt("NUMBER_OF_SENDERS"),
    NumberOfUniqueAccounts = GetRequiredConfigInt("NUMBER_OF_UNIQUE_ACCOUNTS"),
    AcceptPercent = GetRequiredConfigInt("ACCEPT_PERCENT"),
    CancelPercent = GetRequiredConfigInt("NOT_ACCEPTED_CANCEL_PERCENT"),
    MinQuoteRequestDelay = GetRequiredConfigInt("MIN_QUOTE_REQUEST_DELAY"),
    MaxQuoteRequestDelay = GetRequiredConfigInt("MAX_QUOTE_REQUEST_DELAY"),
    MinQuoteQuantity = GetRequiredConfigInt("MIN_QUOTE_QUANTITY"),
    MaxQuoteQuantity = GetRequiredConfigInt("MAX_QUOTE_QUANTITY"),
    MinSellQuantity = GetRequiredConfigInt("MIN_SELL_QUANTITY"),
    MaxSellQuantity = GetRequiredConfigInt("MAX_SELL_QUANTITY"),
    PriceManipulatorInterval = TimeSpan.Parse(GetRequiredConfigString("PRICE_MANIPULATOR_INTERVAL")),
    MinProviderDiscount = GetRequiredConfigInt("MIN_PROVIDER_DISCOUNT"),
    MaxProviderDiscount = GetRequiredConfigInt("MAX_PROVIDER_DISCOUNT"),
    MinPriceCents = GetRequiredConfigInt("MIN_PRICE_CENTS"),
    MaxPriceCents = GetRequiredConfigInt("MAX_PRICE_CENTS")
};
var kafkaOptions = new KafkaOptions()
{
    Servers = GetRequiredConfigString("KAFKA__BOOTSTRAP_SERVERS"),
    GroupId = GetRequiredConfigString("KAFKA__GROUP_ID"),
    LocatorRequestTopic = GetRequiredConfigString("KAFKA__LOCATOR_QUOTE_REQUEST_TOPIC"),
    LocatorResponseTopic = GetRequiredConfigString("KAFKA__LOCATOR_QUOTE_RESPONSE_TOPIC"),
    NumberOfListeners = GetRequiredConfigInt("NUMBER_OF_LISTENERS")
};
var apiOptions = new ApiOptions()
{
    LocatorUrl = GetRequiredConfigString("LOCATOR_BASE_URL"),
    InternalInventoryUrl = GetRequiredConfigString("INTERNAL_INVENTORY_BASE_URL"),
};

var randomHelper = new RandomHelper(emulatorOptions);

//Services
var providerSettingsApi = new ProviderSettingsApi(apiOptions, randomHelper);
var internalInventoryApi = new InternalInventoryApi(apiOptions, randomHelper);
var kafkaSender = new KafkaSender(kafkaOptions);
var quoteRegistrar = new QuoteRegistrar(kafkaSender);
var kafkaListener = new KafkaListener(kafkaOptions, quoteRegistrar);
var statisticsPrinter = new StatisticsPrinter();

var statisticTask = statisticsPrinter.StartAsync(cts.Token);

await providerSettingsApi.InitProviders();
internalInventoryApi.InitInternalInventory();

// var priceManipulator = new PriceManipulator(emulatorOptions, providerSettingsApi, mockProvidersApi, availabilityTypesApi,
//     internalInventoryApi, firmSettingsApi);

// backgroundTasks.Add(priceManipulator.StartAsync(allowNewQuotes.Token));


var emulators = new UserEmulator[emulatorOptions.NumberOfSenders];
for (int i = 0; i < emulatorOptions.NumberOfSenders; i++)
{
    emulators[i] = new UserEmulator(quoteRegistrar, randomHelper, i, emulatorOptions);
}

//Start everything
var listenerTask = kafkaListener.Start(cts.Token);
foreach (var emulator in emulators)
{
    var _ = emulator.StartAsync(cts.Token, allowNewQuotes.Token);
    backgroundTasks.Add(_);
}


SharedData.Log("Ctrl+C to stop");

await Task.Delay(emulatorOptions.EmulationLength, cts.Token);

SharedData.Log($"Before Stop quotes", true);
allowNewQuotes.Cancel();
SharedData.Log($"After Stop quotes", true);
try
{
    Task.WaitAll(backgroundTasks.ToArray(), TimeSpan.FromMinutes(1));
}
catch (AggregateException e)
{
    Console.WriteLine("Exception was thrown during waiting of graceful stop of user emulators");
    Console.WriteLine(e.ToString());
}

cts.Cancel();
try
{
    Task.WaitAll(new[] { listenerTask }, TimeSpan.FromMinutes(1));
}
catch (AggregateException e)
{
    Console.WriteLine("Exception was thrown during waiting of graceful stop of listeners");
    Console.WriteLine(e.ToString());
}

try
{
    if (!statisticTask.IsCanceled)
    {
        Task.WaitAll(new[] { statisticTask }, TimeSpan.FromMinutes(1));
    }
}
catch (AggregateException e)
{
    Console.WriteLine("Exception was thrown during waiting of graceful stop of statistic writer");
    Console.WriteLine(e.ToString());
}


if (SharedData.Cache.Count > 0)
{
    SharedData.Log("We have problems", true);
    foreach (var quoteID in SharedData.Cache.Keys)
    {
        SharedData.Log($"ID: {quoteID}", true);
    }
}

SharedData.Log($"SharedData.Cache.Count: {SharedData.Cache.Count}", true);

StatisticsPrinter.Print();
SharedData.SaveLogToFile();