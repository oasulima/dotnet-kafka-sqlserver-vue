using System;
using System.Linq;

namespace Shared
{
    public class QuoteResponse
    {
        public QuoteResponse()
        {
            Time = DateTime.UtcNow;
        }

        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Symbol { get; set; }

        public QuoteResponseStatusEnum Status { get; set; }
        public string ErrorMessage { get; set; }
        public int ReqQty { get; set; }
        public int? FillQty { get; set; }
        public decimal? Price { get; set; }
        public QuoteSourceInfo[] Sources { get; set; }
        public DateTime Time { get; set; }
        public string DetailsJson { get; set; }

        public override string ToString()
        {
            return $"[ Id: {Id}, AccountId: {AccountId}, Symbol: {Symbol}, Status: {Status}, ErrorMessage: {ErrorMessage}, ReqQty: {ReqQty}, FillQty: {FillQty}, Price: {Price}, Sources: {string.Join(",", this.Sources.Select(x=>x))}, Time: {Time}, DetailsJson: {DetailsJson} ]";
        }
    }
}