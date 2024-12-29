namespace Shared
{
    public class QuoteSourceInfo
    {
        public string Provider { get; set; }
        public string Source { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal DiscountedPrice { get; set; }

        public override string ToString()
        {
            return $"[ Provider: {Provider}, Source: {Source}, Price: {Price}, Qty: {Qty}, DiscountedPrice: {DiscountedPrice}  ]";
        }
    }
}