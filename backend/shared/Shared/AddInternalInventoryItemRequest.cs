namespace Shared
{
    public class AddInternalInventoryItemRequest
    {
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Source { get; set; }
        public CreatingType CreatingType { get; set; }

        public override string ToString()
        {
            return $"{nameof(Symbol)}: [{Symbol}], " +
                   $"{nameof(Quantity)}: [{Quantity}], " +
                   $"{nameof(Price)}: [{Price}], " +
                   $"{nameof(Source)}: [{Source}], " +
                   $"{nameof(CreatingType)}: [{CreatingType}]";
        }
    }
}