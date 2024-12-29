using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Shared
{
    public class ProviderBuyRequest
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Symbol { get; set; }
        public IList<PriceInfo> RequestedAssets { get; set; }
        public DateTime ValidTill { get; set; }
        public string QuoteId { get; set; }

        public override string ToString()
        {
            return $"[ Id: {Id}, AccountId: {AccountId}, Symbol: {Symbol}, RequestedAssets: {JsonConvert.SerializeObject(RequestedAssets)}, ValidTill: {ValidTill}, QuoteId: {QuoteId} ]";
        }
    }
}