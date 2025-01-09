namespace Shared
{
    public class QuoteSourceInfo
    {
        public required string Provider { get; set; }
        public required string Source { get; set; }
        public required decimal Price { get; set; }
        public required int Qty { get; set; }
        public required decimal DiscountedPrice { get; set; }

        public override string ToString()
        {
            return $"[ Provider: {Provider}, Source: {Source}, Price: {Price}, Qty: {Qty}, DiscountedPrice: {DiscountedPrice}  ]";
        }
    }
}
