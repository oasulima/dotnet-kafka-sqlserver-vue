using System;

namespace Shared
{
    public class QuoteRequest
    {
        public enum RequestTypeEnum
        {
            QuoteRequest,
            QuoteAccept,
            QuoteCancel
        }
        
        public QuoteRequest()
        {
            Time = DateTime.UtcNow;
        }

        public string Id { get; set; }
        public RequestTypeEnum RequestType { get; set; }

        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public string AccountId { get; set; }

        public bool AllowPartial { get; set; }

        public bool AutoApprove { get; set; }
        public decimal MaxPriceForAutoApprove { get; set; }
        public DateTime Time { get; set; }

        public override string ToString()
        {
            return $"[ Id: {Id}, RequestType: {RequestType}, Symbol: {Symbol}, Quantity: {Quantity}, AccountId: {AccountId}, AllowPartial: {AllowPartial}, AutoApprove: {AutoApprove}, MaxPriceForAutoApprove: {MaxPriceForAutoApprove}, Time: {Time} ]";
        }
    }
}