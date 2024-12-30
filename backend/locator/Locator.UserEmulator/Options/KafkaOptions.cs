namespace TradeZero.Locator.Emulator.Options;

public class KafkaOptions
{
    public required string Servers { get; init; }
    public required string LocatorRequestTopic { get; init; }
    public required string LocatorResponseTopic { get; init; }
    public required string GroupId { get; init; }
    public int NumberOfListeners { get; init; }
}