using Admin.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Refit;

namespace Admin.API.Controllers;

[ApiController]
public class QuoteController : ControllerBase
{
    private readonly IEventSender _eventSender;

    public QuoteController(IEventSender eventSender, ILocatorApi locatorApi)
    {
        _eventSender = eventSender;
    }

    [HttpPost("/api/quote")]
    public void Quote([FromBody] QuoteRequest quoteRequest)
    {
        _eventSender.SendLocatorQuoteRequest(quoteRequest);
    }
}
