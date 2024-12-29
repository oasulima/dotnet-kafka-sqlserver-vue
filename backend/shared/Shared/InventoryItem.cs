using System;

namespace Shared
{
    public class InternalInventoryItem
    {
        public enum State
        {
            Active,
            Inactive,
            Deleted
        }

        public string Id { get; set; }
        public int Version { get; set; }

        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public int SoldQuantity { get; set; }
        public decimal Price { get; set; }
        public string Source { get; set; }
        public CreatingType CreatingType { get; set; }
        public string? Tag { get; set; }
        public string? CoveredInvItemId { get; set; }

        public State Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: [{Id}], " +
                   $"{nameof(Version)}: [{Version}], " +
                   $"{nameof(Symbol)}: [{Symbol}], " +
                   $"{nameof(Quantity)}: [{Quantity}], " +
                   $"{nameof(SoldQuantity)}: [{SoldQuantity}], " +
                   $"{nameof(Price)}: [{Price}], " +
                   $"{nameof(Source)}: [{Source}], " +
                   $"{nameof(CreatingType)}: [{CreatingType}], " +
                   $"{nameof(Tag)}: [{Tag}], " +
                   $"{nameof(CoveredInvItemId)}: [{CoveredInvItemId}], " +
                   $"{nameof(Status)}: [{Status}], " +
                   $"{nameof(CreatedAt)}: [{CreatedAt}]";
        }
    }
}