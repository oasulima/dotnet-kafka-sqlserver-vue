#nullable enable
namespace Shared.Settings;

public class ProviderSetting
{
    public string ProviderId { get; set; }
    public string Name { get; set; }
    public decimal Discount { get; set; }
    public bool Active { get; set; }

    public string? QuoteRequestTopic { get; set; }
    public string? QuoteResponseTopic { get; set; }

    public string? BuyRequestTopic { get; set; }
    public string? BuyResponseTopic { get; set; }

    public override string ToString()
    {
        return
            $"{nameof(Name)}: {Name}, " +
            $"{nameof(ProviderId)}: {ProviderId}, " +
            $"{nameof(Discount)}: {Discount}, " +
            $"{nameof(Active)}: {Active}";
    }
}

public class ProviderSettingExtended : ProviderSetting
{
    public class AutoDisabledInfo
    {
        public string[]? Symbols { get; set; }
    }

    public AutoDisabledInfo? AutoDisabled { get; set; }
}