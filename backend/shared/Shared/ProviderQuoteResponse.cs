using Newtonsoft.Json;

namespace Shared
{
    public class ProviderQuoteResponse
    {
        public enum StatusEnum
        {
            Pending,
            Ready
        }

        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Symbol { get; set; }
        public IList<PriceInfo> Prices { get; set; }
        public StatusEnum Status { get; set; }
        public string QuoteId { get; set; }

        public override string ToString()
        {
            return $"[ Id: {Id}, AccountId: {AccountId}, Symbol: {Symbol}, Prices: {JsonConvert.SerializeObject(Prices)}, Status: {Status}, QuoteId: {QuoteId}  ]";
        }

    }
}