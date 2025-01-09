namespace Shared
{
    public class PriceInfo
    {
        public required decimal Price { get; set; }
        public required int Quantity { get; set; }
        public required string Source { get; set; }

        public override string ToString()
        {
            return $"[ Price: {Price}, Quantity: {Quantity}, Source: {Source} ]";
        }
    }
}
