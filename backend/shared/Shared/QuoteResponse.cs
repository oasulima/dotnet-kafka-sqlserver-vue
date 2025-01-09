namespace Shared
{
    public class QuoteResponse
    {
        public required string Id { get; init; }
        public required string AccountId { get; set; }
        public required string Symbol { get; set; }

        public QuoteResponseStatusEnum Status { get; set; }
        public string? ErrorMessage { get; set; }
        public int ReqQty { get; set; }
        public int? FillQty { get; set; }
        public decimal? Price { get; set; }
        public required IList<QuoteSourceInfo> Sources { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public string? DetailsJson { get; set; }

        public override string ToString()
        {
            return $"[ Id: {Id}, AccountId: {AccountId}, Symbol: {Symbol}, Status: {Status}, ErrorMessage: {ErrorMessage}, ReqQty: {ReqQty}, FillQty: {FillQty}, Price: {Price}, Sources: {string.Join(",", this.Sources.Select(x => x))}, Time: {Time}, DetailsJson: {DetailsJson} ]";
        }
    }
}
