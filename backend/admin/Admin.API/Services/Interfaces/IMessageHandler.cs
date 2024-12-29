using Shared;

namespace Admin.API.Services.Interfaces;

public interface IMessageHandler
{
    void Handle(QuoteResponse message);
    
    void CleanData();
}