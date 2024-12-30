using LinqToDB.Data;
using Reporting.API.Data.Repositories.Interfaces;

namespace Reporting.API.Data.Repositories;

public class QuoteDetailsRepository : IQuoteDetailsRepository
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public QuoteDetailsRepository(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public void AddResponseDetailsJson(string quoteId, string responseDetailsJson)
    {
        var parameters = new[]
        {
            new DataParameter($"QuoteId", LinqToDB.DataType.VarChar) {Value = quoteId, Size = 100},
            new DataParameter($"ResponseDetailsJson", LinqToDB.DataType.NText) {Value = responseDetailsJson},
        };

        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        linq2Db.ExecuteProc("[dbo].[AddQuoteDetails]", parameters);
    }

    public string? GetResponseDetailsJson(string quoteId)
    {
        var parameters = new[]
        {
            new DataParameter("@QuoteId", LinqToDB.DataType.NVarChar) {Value = quoteId},
        };

        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        return linq2Db.ExecuteProc<Dbo>("[dbo].[GetQuoteDetails]", parameters).ResponseDetailsJson;
    }

    private record Dbo(string QuoteId, string? ResponseDetailsJson);
}