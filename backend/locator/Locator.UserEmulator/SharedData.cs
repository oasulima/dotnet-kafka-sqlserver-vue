using System.Collections.Concurrent;
using Shared;

namespace Locator.UserEmulator;

public static class SharedData
{
    private static long _totalQuotes;
    private static long _totalAccepts;
    private static long _totalCancels;
    private static long _totalIgnores;
    private static long _totalFailed;

    public static long TotalQuotes => Interlocked.Read(ref _totalQuotes);
    public static long TotalAccepts => Interlocked.Read(ref _totalAccepts);
    public static long TotalCancels => Interlocked.Read(ref _totalCancels);
    public static long TotalIgnores => Interlocked.Read(ref _totalIgnores);
    public static long TotalFailed => Interlocked.Read(ref _totalFailed);

    public static void IncrementQuotes() => Interlocked.Increment(ref _totalQuotes);

    public static void IncrementAccepts() => Interlocked.Increment(ref _totalAccepts);

    public static void IncrementCancels() => Interlocked.Increment(ref _totalCancels);

    public static void IncrementIgnores() => Interlocked.Increment(ref _totalIgnores);

    public static void IncrementFailed() => Interlocked.Increment(ref _totalFailed);

    private static readonly ConcurrentBag<LogRecord> log = new();

    public static readonly ConcurrentDictionary<string, TaskCompletionSource<QuoteResponse>> Cache =
        new();

    public static void Log(string str, bool toConsole = false)
    {
        var time = DateTime.Now;
        var message = $"{time:O}: {str}";
        log.Add(new LogRecord(time, message));

        if (toConsole)
        {
            Console.WriteLine(message);
        }
    }

    public static void SaveLogToFile()
    {
        Directory.CreateDirectory("logs");
        File.WriteAllLines(
            @$"logs/useremulator-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt",
            log.OrderBy(x => x.date).Select(x => x.message)
        );
    }

    record LogRecord(DateTime date, string message);
}
