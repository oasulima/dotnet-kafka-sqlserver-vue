using Microsoft.AspNetCore.Mvc;
using Reporting.API.Data.Repositories.Interfaces;
using Reporting.API.Services.Interfaces;
using Reporting.API.Utility;
using Shared.Quote;
using Shared.Reporting;
using Shared.Reporting.Requests;

namespace Reporting.API.Controllers;

[ApiController]
public class ReportDataController : ControllerBase
{
    private readonly ILocatesReportDataService _locatesReportDataService;

    public ReportDataController(
        ILocatesReportDataService locatesReportDataService)
    {
        _locatesReportDataService = locatesReportDataService;
    }

    [HttpPost("/api/report/data/locates")]
    public IList<LocatesReportData> GetLocatesReportData([FromBody] LocatesReportDataRequest locatesReportData)
    {
        return _locatesReportDataService.GetLocatesReportData(locatesReportData);

    }

    [HttpGet("/api/report/data/quote/responses")]
    public IList<LocatorQuoteResponse> GetLocatorQuoteResponses([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int take, [FromQuery] int skip)
    {
        return _locatesReportDataService.GetLocatorQuoteResponses(from, to, take, skip);
    }
}