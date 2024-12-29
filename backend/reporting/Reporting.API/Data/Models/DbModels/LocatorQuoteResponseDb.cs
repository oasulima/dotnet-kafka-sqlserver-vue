﻿namespace Reporting.API.Data.Models.DbModels;

public class LocatorQuoteResponseDb
{
    public string Id { get; set; }
    public string AccountId { get; set; }
    public string Symbol { get; set; }

    public byte Status { get; set; }
    public DateTime Time { get; set; }
    public string? ErrorMessage { get; set; }
    public int ReqQty { get; set; }
    public int FillQty { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
    public string Source { get; set; }// = string.Empty; 
    public QuoteSourceInfoDb[] Sources { get; set; }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, " +
               $"{nameof(AccountId)}: {AccountId}, " +
               $"{nameof(Symbol)}: {Symbol}, " +
               $"{nameof(Status)}: {Status}, " +
               $"{nameof(Time)}: {Time}, " +
               $"{nameof(ErrorMessage)}: {ErrorMessage}, " +
               $"{nameof(ReqQty)}: {ReqQty}, " +
               $"{nameof(FillQty)}: {FillQty}, " +
               $"{nameof(Price)}: {Price}, " +
               $"{nameof(Source)}: {Source}, " +
               $"{nameof(Sources)}: [{string.Join(", ", Sources.Select(s => s.ToString()))}]";
    }
}