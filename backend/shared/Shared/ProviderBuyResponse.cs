using Newtonsoft.Json;

namespace Shared
{
    public class ProviderBuyResponse
    {
        public enum StatusEnum
        {
            Cancelled,
            Failed,
            NoInventory,
            Fulfilled,
            Partial
        }

        public string Id { get; set; }
        public string Symbol { get; set; }
        public string AccountId { get; set; }
        public IList<PriceInfo> BoughtAssets { get; set; }
        public StatusEnum Status { get; set; }
        public string RejectCode { get; set; }
        public string QuoteId { get; set; }

        public override string ToString()
        {
            return $"[ Id: {Id}, Symbol: {Symbol}, AccountId: {AccountId}, BoughtAssets: {JsonConvert.SerializeObject(BoughtAssets)}, Status: {Status}, RejectCode: {RejectCode}, QuoteId: {QuoteId} ]";
        }
    }
}