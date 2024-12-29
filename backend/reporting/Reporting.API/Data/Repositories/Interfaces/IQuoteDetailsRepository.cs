namespace Reporting.API.Data.Repositories.Interfaces;

public interface IQuoteDetailsRepository
{
    void AddResponseDetailsJson(string quoteId, string responseDetailsJson);
    string? GetResponseDetailsJson(string quoteId);
}