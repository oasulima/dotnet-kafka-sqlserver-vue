namespace Shared
{
    public class AddInternalInventoryItemRequest
    {
        public required string Symbol { get; set; }
        public required int Quantity { get; set; }
        public required decimal Price { get; set; }
        public required string Source { get; set; }
        public required CreatingType CreatingType { get; set; }

        public override string ToString()
        {
            return $"{nameof(Symbol)}: [{Symbol}], "
                + $"{nameof(Quantity)}: [{Quantity}], "
                + $"{nameof(Price)}: [{Price}], "
                + $"{nameof(Source)}: [{Source}], "
                + $"{nameof(CreatingType)}: [{CreatingType}]";
        }
    }
}
