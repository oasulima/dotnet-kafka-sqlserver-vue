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

var connectionString = args.FirstOrDefault() ?? GetRequiredConfigString("CONNECTION_STRING");

EnsureDatabase.For.SqlDatabase(connectionString);

var upgrader = DeployChanges
    .To.SqlDatabase(connectionString, "dbo")
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
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
