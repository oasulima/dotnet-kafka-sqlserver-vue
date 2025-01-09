namespace Shared
{
    public class ProviderQuoteResponse
    {
        public enum StatusEnum
        {
            Pending,
            Ready,
        }

        public required string Id { get; set; }
        public required string AccountId { get; set; }
        public required string Symbol { get; set; }
        public required IList<PriceInfo> Prices { get; set; }
        public StatusEnum Status { get; set; }
        public required string QuoteId { get; set; }

        public override string ToString()
        {
            return $"[ Id: {Id}, AccountId: {AccountId}, Symbol: {Symbol}, Prices: {Converter.Serialize(Prices)}, Status: {Status}, QuoteId: {QuoteId}  ]";
        }
    }
}
