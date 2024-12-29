using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Refit;

namespace Admin.API.Controllers;

[ApiController]
public class QuoteDetailsController : ControllerBase
{
    private readonly IReportingApi _reportingApi;

    public QuoteDetailsController(IReportingApi reportingApi)
    {
        _reportingApi = reportingApi;
    }

    [HttpGet("api/QuoteDetails")]
    public async Task<object?> Get([FromQuery] string quoteId)
    {
        var result = await _reportingApi.GetLocatesReportData(quoteId);
        return JsonConvert.DeserializeObject(result);
    }
}