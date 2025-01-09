namespace Shared
{
    public class ProviderBuyRequest
    {
        public required string Id { get; init; }
        public required string AccountId { get; set; }
        public required string Symbol { get; set; }
        public required IList<PriceInfo> RequestedAssets { get; set; }
        public DateTime ValidTill { get; set; }
        public required string QuoteId { get; set; }

        public override string ToString()
        {
            return $"[ Id: {Id}, AccountId: {AccountId}, Symbol: {Symbol}, RequestedAssets: {Converter.Serialize(RequestedAssets)}, ValidTill: {ValidTill}, QuoteId: {QuoteId} ]";
        }
    }
}
