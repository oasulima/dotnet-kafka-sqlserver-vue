using Locator.UserEmulator;
using Locator.UserEmulator.Options;

namespace Locator.UserEmulator.Services;

public class PriceManipulator
{
    private readonly EmulatorOptions _emulatorOptions;
    private readonly InternalInventoryApi _internalInventoryApi;

    public PriceManipulator(
        EmulatorOptions emulatorOptions,
        InternalInventoryApi internalInventoryApi
    )
    {
        _emulatorOptions = emulatorOptions;
        _internalInventoryApi = internalInventoryApi;
    }

    public Task StartAsync(CancellationToken cts)
    {
        return DoWorkAsync(cts);
    }

    private async Task DoWorkAsync(CancellationToken cts)
    {
        SharedData.Log("Start price manipulator");

        ChangeData();

        using var timer = new PeriodicTimer(_emulatorOptions.PriceManipulatorInterval);

        try
        {
            while (!cts.IsCancellationRequested && await timer.WaitForNextTickAsync(cts))
            {
                ChangeData();
            }
        }
        catch (TaskCanceledException) { }

        SharedData.Log($"Stop price manipulator");
    }

    private void ChangeData()
    {
        SharedData.Log("change settings");
        _internalInventoryApi.InitInternalInventory();
    }
}
