using Microsoft.AspNetCore.Mvc;
using Reporting.API.Data.Repositories.Interfaces;

namespace Reporting.API.Controllers;

[ApiController]
public class QuoteDetailsController : ControllerBase
{
    private readonly IQuoteDetailsRepository _quoteDetailsRepository;

    public QuoteDetailsController(IQuoteDetailsRepository quoteDetailsRepository)
    {
        _quoteDetailsRepository = quoteDetailsRepository;
    }

    [HttpGet("/api/QuoteDetails")]
    public string GetLocatesReportData([FromQuery] string quoteId)
    {
        return _quoteDetailsRepository.GetResponseDetailsJson(quoteId) ?? "null";
    }
}