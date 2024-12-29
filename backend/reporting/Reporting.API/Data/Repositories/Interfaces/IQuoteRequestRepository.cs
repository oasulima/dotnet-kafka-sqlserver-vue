using Reporting.API.Data.Models.DbModels;

namespace Reporting.API.Data.Repositories.Interfaces;

public interface IQuoteRequestRepository
{
    void AddQuoteRequest(QuoteRequestDb quoteRequest);
}
