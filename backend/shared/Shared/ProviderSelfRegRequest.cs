namespace Shared
{
    public class ProviderSelfRegRequest
    {
        public required string Name { get; set; }
        public required string Id { get; set; }

        public required string QuoteRequestTopic { get; set; }
        public required string QuoteResponseTopic { get; set; }

        public required string BuyRequestTopic { get; set; }
        public required string BuyResponseTopic { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: [{Name}]"
                + $"{nameof(Id)}: [{Id}]"
                + $"{nameof(QuoteRequestTopic)}: [{QuoteRequestTopic}]"
                + $"{nameof(QuoteResponseTopic)}: [{QuoteResponseTopic}]"
                + $"{nameof(BuyRequestTopic)}: [{BuyRequestTopic}]"
                + $"{nameof(BuyResponseTopic)}: [{BuyResponseTopic}]";
        }
    }
}
