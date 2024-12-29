using LinqToDB.Data;
using Reporting.API.Data.Repositories.Interfaces;

namespace Reporting.API.Data.Repositories;

public class QuoteDetailsRepository : IQuoteDetailsRepository
{
    private readonly DbConnection linq2Db;

    public QuoteDetailsRepository(DbConnection linq2Db)
    {
        this.linq2Db = linq2Db;
    }

    public void AddResponseDetailsJson(string quoteId, string responseDetailsJson)
    {
        var parameters = new[]
        {
            new DataParameter($"QuoteId", LinqToDB.DataType.VarChar) {Value = quoteId, Size = 100},
            new DataParameter($"ResponseDetailsJson", LinqToDB.DataType.NText) {Value = responseDetailsJson},
        };

        linq2Db.ExecuteProc("[dbo].[AddQuoteDetails]", parameters);
    }

    public string? GetResponseDetailsJson(string quoteId)
    {
        var parameters = new[]
        {
            new DataParameter("@QuoteId", LinqToDB.DataType.NVarChar) {Value = quoteId},
        };

        return linq2Db.ExecuteProc<Dbo>("[dbo].[GetQuoteDetails]", parameters).ResponseDetailsJson;
    }

    private record Dbo(string QuoteId, string? ResponseDetailsJson);
}