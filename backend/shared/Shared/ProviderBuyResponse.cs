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
            Partial,
        }

        public required string Id { get; set; }
        public required string Symbol { get; set; }
        public required string AccountId { get; set; }
        public required IList<PriceInfo> BoughtAssets { get; set; }
        public required StatusEnum Status { get; set; }
        public required string QuoteId { get; set; }

        public override string ToString()
        {
            return $"[ Id: {Id}, Symbol: {Symbol}, AccountId: {AccountId}, BoughtAssets: {Converter.Serialize(BoughtAssets)}, Status: {Status}, QuoteId: {QuoteId} ]";
        }
    }
}
