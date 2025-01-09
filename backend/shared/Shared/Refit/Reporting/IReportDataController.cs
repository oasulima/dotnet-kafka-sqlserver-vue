using Refit;
using Shared.Quote;
using Shared.Reporting;
using Shared.Reporting.Requests;

namespace Shared.Refit.Reporting;

public interface IReportDataController
{
    [Post("/api/report/data/locates")]
    public Task<IList<LocatesReportData>> GetLocatesReportData(
        [Body] LocatesReportDataRequest locatesReportData
    );

    [Get("/api/report/data/quote/responses")]
    public Task<IList<LocatorQuoteResponse>> GetLocatorQuoteResponses(
        DateTime from,
        DateTime to,
        int take,
        int skip
    );
}
