namespace Shared.Settings;

public class ProviderSetting
{
    public required string ProviderId { get; set; }
    public required string Name { get; set; }
    public required decimal Discount { get; set; }
    public required bool Active { get; set; }

    public string? QuoteRequestTopic { get; set; }
    public string? QuoteResponseTopic { get; set; }

    public string? BuyRequestTopic { get; set; }
    public string? BuyResponseTopic { get; set; }

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, "
            + $"{nameof(ProviderId)}: {ProviderId}, "
            + $"{nameof(Discount)}: {Discount}, "
            + $"{nameof(Active)}: {Active}";
    }
}

public class ProviderSettingExtended : ProviderSetting
{
    public class AutoDisabledInfo
    {
        public IList<string>? Symbols { get; set; }
    }

    public AutoDisabledInfo? AutoDisabled { get; set; }
}
