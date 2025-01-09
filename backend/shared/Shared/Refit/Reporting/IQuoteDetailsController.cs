using Refit;

namespace Shared.Refit.Reporting;

public interface IQuoteDetailsController
{
    [Get("/api/QuoteDetails")]
    Task<string> GetLocatesReportData(string quoteId);
}
