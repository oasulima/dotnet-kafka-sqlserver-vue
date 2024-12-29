namespace Shared
{
    public class ProviderQuoteRequest
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public DateTime ValidTill { get; set; }
        public string QuoteId { get; set; }

        public override string ToString()
        {
            return $"[ Id: {Id}, AccountId: {AccountId}, Symbol: {Symbol}, Quantity: {Quantity}, ValidTill: {ValidTill}, QuoteId: {QuoteId} ]";
        }
    }
}