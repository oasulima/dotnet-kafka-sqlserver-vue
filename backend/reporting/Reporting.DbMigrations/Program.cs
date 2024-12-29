using System.Collections;
using System.Reflection;
using DbUp;

static string GetRequiredConfigString(string parameterName)
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

var connectionString =
    args.FirstOrDefault()
    ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");

var variables = new Dictionary<string, string>();
foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
{
    variables.Add(e.Key.ToString(), e.Value.ToString());
}

var ensureDbCreated = Environment.GetEnvironmentVariable("ENSURE_DB_CREATED");
if (ensureDbCreated != null && bool.TryParse(ensureDbCreated, out bool flag) && flag)
{
    EnsureDatabase.For.SqlDatabase(connectionString);
}

var upgrader =
    DeployChanges.To
        .SqlDatabase(connectionString, "dbo")
        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        .LogToConsole()
        .WithVariables(variables)
        .WithTransactionPerScript()
        .WithExecutionTimeout(TimeSpan.FromMinutes(30))
        .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();
return 0;