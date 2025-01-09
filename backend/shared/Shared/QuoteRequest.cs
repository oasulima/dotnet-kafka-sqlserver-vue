namespace Shared
{
    public class QuoteRequest
    {
        public enum RequestTypeEnum
        {
            QuoteRequest,
            QuoteAccept,
            QuoteCancel,
        }

        public required string Id { get; set; }
        public required RequestTypeEnum RequestType { get; set; }

        public required string Symbol { get; set; }
        public required int Quantity { get; set; }
        public required string AccountId { get; set; }

        public required bool AllowPartial { get; set; }

        public required bool AutoApprove { get; set; }
        public required decimal MaxPriceForAutoApprove { get; set; }
        public required DateTime Time { get; set; } = DateTime.UtcNow;

        public override string ToString()
        {
            return $"[ Id: {Id}, RequestType: {RequestType}, Symbol: {Symbol}, Quantity: {Quantity}, AccountId: {AccountId}, AllowPartial: {AllowPartial}, AutoApprove: {AutoApprove}, MaxPriceForAutoApprove: {MaxPriceForAutoApprove}, Time: {Time} ]";
        }
    }
}
