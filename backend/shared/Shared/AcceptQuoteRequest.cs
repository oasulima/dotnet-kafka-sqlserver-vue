using System.Security.Cryptography.X509Certificates;

namespace Shared
{
    public class AcceptQuoteRequest
    {

        public long QuoteId { get; set; }

        public decimal? MaxPriceToAccept { get; set; }

        public override string ToString()
        {
            return $"{nameof(QuoteId)}: [{QuoteId}], " +
                   $"{nameof(MaxPriceToAccept)}: [{MaxPriceToAccept}]";
        }
    }
}