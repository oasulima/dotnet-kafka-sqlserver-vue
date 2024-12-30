using Reporting.API.Data.Models.DbModels;
using Reporting.API.Data.Repositories.Interfaces;
using LinqToDB.Data;

namespace Reporting.API.Data.Repositories;

public class QuoteRequestRepository : IQuoteRequestRepository
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public QuoteRequestRepository(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public QuoteRequestDb[] GetQuoteRequests(DateTime? from, DateTime? to)
    {
        var parameters = new[]
        {
            new DataParameter("@From", LinqToDB.DataType.DateTime) {Value = from},
            new DataParameter("@To", LinqToDB.DataType.DateTime) {Value = to},
        };

        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        return linq2Db.QueryProc<QuoteRequestDb>("[dbo].[GetQuoteRequests]", parameters).ToArray();
    }

    public void AddQuoteRequest(QuoteRequestDb quoteRequest)
    {
        var parameters = new[]
        {
            new DataParameter("@Id", quoteRequest.Id),
            new DataParameter("@AccountId", quoteRequest.AccountId),
            new DataParameter("@RequestType", quoteRequest.RequestType.ToString()),
            new DataParameter("@Symbol", quoteRequest.Symbol),
            new DataParameter("@Quantity", quoteRequest.Quantity),
            new DataParameter("@AllowPartial", quoteRequest.AllowPartial),
            new DataParameter("@AutoApprove", quoteRequest.AutoApprove),
            new DataParameter("@MaxPriceForAutoApprove", quoteRequest.MaxPriceForAutoApprove),
            new DataParameter("@Time", quoteRequest.Time),
        };

        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        linq2Db.ExecuteProc("[dbo].[AddQuoteRequest]", parameters);
    }
}