namespace Reporting.API.Data.Models.DbModels;

public class QuoteSourceInfoDb
{
    public string? Provider { get; set; }
    public string Source { get; set; }
    public decimal Price { get; set; }
    public int Qty { get; set; }
    public decimal DiscountedPrice { get; set; }

    public override string ToString()
    {
        return $"{{ {nameof(Provider)}: {Provider}, " +
               $"{nameof(Source)}: {Source}, " +
               $"{nameof(Price)}: {Price}, " +
               $"{nameof(Qty)}: {Qty}, " +
               $"{nameof(DiscountedPrice)}: {DiscountedPrice} }}";
    }
}