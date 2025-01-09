using Microsoft.AspNetCore.Mvc;
using Shared.Refit;
using Shared.Reporting;
using Shared.Reporting.Requests;

namespace Admin.API.Controllers;

[ApiController]
public class ReportController : ControllerBase
{
    private readonly IReportingApi _reportingApi;

    public ReportController(IReportingApi reportingApi)
    {
        _reportingApi = reportingApi;
    }

    [HttpPost("/api/report/locates")]
    public Task<IList<LocatesReportData>> GetLocatesReportData(
        [FromBody] LocatesReportDataRequest request
    )
    {
        return _reportingApi.GetLocatesReportData(request);
    }
}
