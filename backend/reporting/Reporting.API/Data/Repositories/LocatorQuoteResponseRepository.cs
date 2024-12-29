using System.Data;
using LinqToDB.Data;
using Newtonsoft.Json;
using Reporting.API.Data.Extensions;
using Reporting.API.Data.Models.DbModels;
using Reporting.API.Data.Models.DbParams;
using Reporting.API.Data.Repositories.Interfaces;
using Shared;

namespace Reporting.API.Data.Repositories;

public class LocatorQuoteResponseRepository : ILocatorQuoteResponseRepository
{
    private readonly DbConnection linq2Db;

    public LocatorQuoteResponseRepository(DbConnection linq2Db)
    {
        this.linq2Db = linq2Db;
    }

    public List<LocatorQuoteResponseDb> GetLocatorQuoteResponses(DateTime from, DateTime to)
    {
        var result = new List<LocatorQuoteResponseDb>();
        var take = 100_000;
        var skip = 0;
        LocatorQuoteResponseDb[] responses;
        do
        {
            responses = GetLocatorQuoteResponses(from, to, take, skip);
            skip += take;
            result.AddRange(responses);
        } while (responses.Count() > 0);

        return result;
    }

    public LocatorQuoteResponseDb[] GetLocatorQuoteResponses(DateTime from, DateTime to, int take, int skip)
    {
        var parameters = new[]
        {
            new DataParameter("@Skip", LinqToDB.DataType.Int32) {Value = skip},
            new DataParameter("@Take", LinqToDB.DataType.Int32) {Value = take},
            new DataParameter("@From", LinqToDB.DataType.DateTime) {Value = from},
            new DataParameter("@To", LinqToDB.DataType.DateTime) {Value = to}
        };
        return linq2Db.QueryProc<LocatorQuoteResponseDb>("[dbo].[GetLocatorQuoteResponses]", parameters).ToArray();
    }

    public LocatesReportDataDb[] GetLocatesReportData(GetLocatesReportDataDbParams @params)
    {
        var symbol = TrimOrNullIfWhiteSpace(@params.Symbol);

        var parameters = new[]
        {
            new DataParameter("@Skip", LinqToDB.DataType.Int32) {Value = @params.Skip},
            new DataParameter("@Take", LinqToDB.DataType.Int32) {Value = @params.Take},
            new DataParameter("@OrderBy", LinqToDB.DataType.VarChar) {Value = @params.OrderBy},
            new DataParameter("@From", LinqToDB.DataType.DateTime) {Value = @params.From ?? (object) DBNull.Value},
            new DataParameter("@To", LinqToDB.DataType.DateTime) {Value = @params.To ?? (object) DBNull.Value},
            new DataParameter("@Symbol", LinqToDB.DataType.VarChar) {Value = symbol ?? (object) DBNull.Value},
            new DataParameter("@Status", LinqToDB.DataType.Byte) {Value = @params.Status ?? (object) DBNull.Value},
            new DataParameter("@AccountId", LinqToDB.DataType.VarChar)
                {Value = @params.AccountId ?? (object) DBNull.Value},
            new DataParameter("@ProviderId", LinqToDB.DataType.VarChar)
                {Value = @params.ProviderId ?? (object) DBNull.Value},
        };

        return linq2Db.QueryProc<LocatesReportDataDb>("[dbo].[GetLocatesReportData]", parameters).ToArray();
    }

    public SymbolQuantityDb[] GetQuoteSymbolQuantities(DateTime from, DateTime to,
        IReadOnlyCollection<string>? includeProviders = null, IReadOnlyCollection<string>? excludeProviders = null,
        IReadOnlyCollection<QuoteResponseStatusEnum>? statuses = null,
        IReadOnlyCollection<string>? excludeAccountIdsJson = null, IReadOnlyCollection<string>? includeAccountIdsJson = null)
    {
        var parameters = new List<DataParameter>
        {
            new DataParameter("@From", LinqToDB.DataType.DateTime) {Value = from},
            new DataParameter("@To", LinqToDB.DataType.DateTime) {Value = to}
        };

        parameters.AddAsJsonIfNotEmpty("@IncludeProvidersJson", includeProviders);
        parameters.AddAsJsonIfNotEmpty("@ExcludeProvidersJson", excludeProviders);
        parameters.AddAsJsonIfNotEmpty("@StatusesJson", statuses?.Select(x => (int)x).ToArray());
        parameters.AddAsJsonIfNotEmpty("@ExcludeAccountIdsJson", excludeAccountIdsJson);
        parameters.AddAsJsonIfNotEmpty("@IncludeAccountIdsJson", includeAccountIdsJson);

        return linq2Db.QueryProc<SymbolQuantityDb>("[dbo].[GetQuoteSymbolQuantities]", parameters.ToArray()).ToArray();
    }

    public void Add(LocatorQuoteResponseDb model)
    {
        var parameters = new[]
        {
            new DataParameter($"RecordID", LinqToDB.DataType.VarChar)
                {Value = Guid.NewGuid().ToString(), Size= 100},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.Id)}", LinqToDB.DataType.VarChar)
                {Value = model.Id, Size= 100},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.AccountId)}", LinqToDB.DataType.VarChar)
                {Value = model.AccountId, Size= 100},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.Symbol)}", LinqToDB.DataType.VarChar)
                {Value = model.Symbol, Size= 100},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.Status)}", LinqToDB.DataType.Int16)
                {Value = model.Status},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.ErrorMessage)}", LinqToDB.DataType.VarChar)
                {Value = model.ErrorMessage},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.Time)}", LinqToDB.DataType.DateTime)
                {Value = model.Time},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.ReqQty)}", LinqToDB.DataType.Int32)
                {Value = model.ReqQty},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.FillQty)}", LinqToDB.DataType.Int32)
                {Value = model.FillQty},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.Price)}", LinqToDB.DataType.Decimal)
                {Value = model.Price, Precision = 10, Scale = 5},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.DiscountedPrice)}", LinqToDB.DataType.Decimal)
                {Value = model.DiscountedPrice, Precision = 10, Scale = 5},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.Source)}", LinqToDB.DataType.VarChar)
                {Value = model.Source},
            new DataParameter($"@{nameof(LocatorQuoteResponseDb.Sources)}", LinqToDB.DataType.VarChar)
                {Value = JsonConvert.SerializeObject(model.Sources)}
        };

        linq2Db.ExecuteProc("[dbo].[AddLocatorQuoteResponse]", parameters);
    }

    private static string? TrimOrNullIfWhiteSpace(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return null;
        }

        return s.Trim();
    }
}