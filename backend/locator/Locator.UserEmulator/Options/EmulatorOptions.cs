namespace Locator.UserEmulator.Options;

public class EmulatorOptions
{
    public TimeSpan EmulationLength { get; init; }
    public int NumberOfSymbols { get; init; }
    public int NumberOfSenders { get; init; }
    public int NumberOfUniqueAccounts { get; init; }
    public int AcceptPercent { get; init; }
    public int CancelPercent { get; init; }
    public int MinQuoteRequestDelay { get; init; }
    public int MaxQuoteRequestDelay { get; init; }
    public int MinQuoteQuantity { get; init; }
    public int MaxQuoteQuantity { get; init; }
    public TimeSpan PriceManipulatorInterval { get; init; }
    public int MinProviderDiscount { get; init; }
    public int MaxProviderDiscount { get; init; }
    public int MinPriceCents { get; init; }
    public int MaxPriceCents { get; init; }
    public int MinSellQuantity { get; init; }
    public int MaxSellQuantity { get; init; }
}

public class ProviderOptions
{
    public required string Name { get; init; }
    public required string Id { get; init; }
}