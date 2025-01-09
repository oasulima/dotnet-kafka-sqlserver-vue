namespace Shared
{
    public class InternalInventoryItem
    {
        public enum State
        {
            Active,
            Inactive,
            Deleted,
        }

        public required string Id { get; init; }
        public required int Version { get; set; }

        public required string Symbol { get; set; }
        public required int Quantity { get; set; }
        public required int SoldQuantity { get; set; }
        public required decimal Price { get; set; }
        public required string Source { get; set; }
        public required CreatingType CreatingType { get; set; }
        public string? Tag { get; set; }
        public string? CoveredInvItemId { get; set; }

        public required State Status { get; set; }
        public required DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: [{Id}], "
                + $"{nameof(Version)}: [{Version}], "
                + $"{nameof(Symbol)}: [{Symbol}], "
                + $"{nameof(Quantity)}: [{Quantity}], "
                + $"{nameof(SoldQuantity)}: [{SoldQuantity}], "
                + $"{nameof(Price)}: [{Price}], "
                + $"{nameof(Source)}: [{Source}], "
                + $"{nameof(CreatingType)}: [{CreatingType}], "
                + $"{nameof(Tag)}: [{Tag}], "
                + $"{nameof(CoveredInvItemId)}: [{CoveredInvItemId}], "
                + $"{nameof(Status)}: [{Status}], "
                + $"{nameof(CreatedAt)}: [{CreatedAt}]";
        }
    }
}
