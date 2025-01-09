using Locator.UserEmulator;

namespace Locator.UserEmulator.Services;

public class StatisticsPrinter
{
    public Task StartAsync(CancellationToken cts)
    {
        return DoWorkAsync(cts);
    }

    private async Task DoWorkAsync(CancellationToken cts)
    {
        SharedData.Log("Start Statistics Printer");

        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync(cts))
            {
                Print();
            }
        }
        catch (TaskCanceledException)
        {
            //noop
        }

        SharedData.Log($"Stop Statistics Printer");
    }

    public static void Print()
    {
        SharedData.Log(
            $"Quotes: {SharedData.TotalQuotes}; Accepted: {SharedData.TotalAccepts}; " +
            $"Cancelled: {SharedData.TotalCancels}; Failed: {SharedData.TotalFailed}; " +
            $"Ignored: {SharedData.TotalIgnores}; InCache: {SharedData.Cache.Count}",
            true);
    }
}