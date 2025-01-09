using Locator.UserEmulator.Options;

namespace Locator.UserEmulator.Services;

public class RandomHelper
{
    private readonly EmulatorOptions emulatorOptions;
    private readonly string[] symbols;
    private readonly string[] sources;

    public RandomHelper(EmulatorOptions emulatorOptions)
    {
        this.emulatorOptions = emulatorOptions;
        symbols = GenerateSymbols(emulatorOptions.NumberOfSymbols);
        sources = ["UES1", "UES2", "UES3"];
    }

    private static string[] GenerateSymbols(int count)
    {
        count = Math.Min(count, Symbols.AllSymbol.Length);
        var result = new string[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = Symbols.AllSymbol[i]; //$"S{i + 1}";
        }

        return result;
    }

    public string[] GetSymbols()
    {
        return symbols;
    }

    public string[] GetSources()
    {
        return sources;
    }

    public string GetRandomSource()
    {
        var index = Random.Shared.Next(0, sources.Length);
        return sources[index];
    }

    public string GetRandomSymbol()
    {
        var index = Random.Shared.Next(0, symbols.Length);
        return symbols[index];
    }

    public int GetRandomPercent100() => Random.Shared.Next(1, 101);

    public decimal GetRandomProviderDiscount() =>
        Random.Shared.Next(
            emulatorOptions.MinProviderDiscount,
            emulatorOptions.MaxProviderDiscount + 1
        ) / 100M;

    public bool ShouldAccept()
    {
        return GetRandomPercent100() <= emulatorOptions.AcceptPercent;
    }

    public bool ShouldCancel()
    {
        return GetRandomPercent100() <= emulatorOptions.CancelPercent;
    }

    public int GetDelayBeforeQuote()
    {
        return Random.Shared.Next(
            emulatorOptions.MinQuoteRequestDelay,
            emulatorOptions.MaxQuoteRequestDelay
        );
    }

    public int GetRandomQuoteQuantity()
    {
        return Random.Shared.Next(
                emulatorOptions.MinQuoteQuantity,
                emulatorOptions.MaxQuoteQuantity
            )
            / 100
            * 100;
    }

    public int GetRandomSellQuantity()
    {
        return Random.Shared.Next(emulatorOptions.MinSellQuantity, emulatorOptions.MaxSellQuantity)
            / 100
            * 100;
    }

    public bool GetRandomBool() => Random.Shared.Next(2) == 1;

    public decimal GetRandomMockPrice() =>
        Random.Shared.Next(emulatorOptions.MinPriceCents, emulatorOptions.MaxPriceCents + 1) / 100M;

    public int GetRandomPriority() => Random.Shared.Next(1, 5);
}
