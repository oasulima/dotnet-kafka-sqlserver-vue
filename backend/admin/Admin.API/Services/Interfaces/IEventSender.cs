using Shared;

namespace Admin.API.Services.Interfaces;

public interface IEventSender
{
    void SendLocatorQuoteRequest(QuoteRequest quoteRequest);
    void SendEnableProviderCommand(string providerId);
}