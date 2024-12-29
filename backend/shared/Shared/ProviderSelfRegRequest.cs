namespace Shared
{
    public class ProviderSelfRegRequest
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public string QuoteRequestTopic { get; set; }
        public string QuoteResponseTopic { get; set; }

        public string BuyRequestTopic { get; set; }
        public string BuyResponseTopic { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: [{Name}]" +
                   $"{nameof(Id)}: [{Id}]" +
                   $"{nameof(QuoteRequestTopic)}: [{QuoteRequestTopic}]" +
                   $"{nameof(QuoteResponseTopic)}: [{QuoteResponseTopic}]" +
                   $"{nameof(BuyRequestTopic)}: [{BuyRequestTopic}]" +
                   $"{nameof(BuyResponseTopic)}: [{BuyResponseTopic}]";
        }
    }
}