namespace Shared
{
    public class PriceInfo
    {
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Source { get; set; }

        public string OriginalSource { get; set; }
        public decimal? OriginalPrice { get; set; }

        public string AccountId { get; set; }

        public override string ToString()
        {
            return $"[ Price: {Price}, Quantity: {Quantity}, Source: {Source}, OriginalSource: {OriginalSource}, OriginalPrice: {OriginalPrice}, AccountId: {AccountId}  ]";
        }
    }
}