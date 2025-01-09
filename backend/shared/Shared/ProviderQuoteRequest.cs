namespace Shared
{
    public class ProviderQuoteRequest
    {
        public required string Id { get; init; }
        public required string AccountId { get; set; }
        public required string Symbol { get; set; }
        public int Quantity { get; set; }
        public DateTime ValidTill { get; set; }
        public required string QuoteId { get; set; }

        public override string ToString()
        {
            return $"[ Id: {Id}, AccountId: {AccountId}, Symbol: {Symbol}, Quantity: {Quantity}, ValidTill: {ValidTill}, QuoteId: {QuoteId} ]";
        }
    }
}
