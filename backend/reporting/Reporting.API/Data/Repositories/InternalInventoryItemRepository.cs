using System.Data;
using LinqToDB;
using LinqToDB.Data;
using Reporting.API.Data.Extensions;
using Reporting.API.Data.Models.DbModels;
using Reporting.API.Data.Repositories.Interfaces;
using Shared;

namespace Reporting.API.Data.Repositories;

public class InternalInventoryItemRepository : IInternalInventoryItemRepository
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public InternalInventoryItemRepository(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public InternalInventoryItemDb[] GetInternalInventoryItems(
        DateTime? from = null,
        DateTime? to = null,
        string? symbol = null,
        CreatingType? creatingType = null,
        InternalInventoryItem.State? status = null
    )
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        return linq2Db
            .QueryProc<InternalInventoryItemDb>(
                "[dbo].[GetInternalInventoryItems]",
                [
                    new DataParameter("@From", from, DataType.DateTime),
                    new DataParameter("@To", to, DataType.DateTime),
                    new DataParameter("@Symbol", symbol, DataType.VarChar),
                    new DataParameter("@CreatingType", creatingType, DataType.VarChar),
                    new DataParameter("@Status", status, DataType.VarChar),
                ]
            )
            .ToArray();
    }

    public InternalInventoryItemDb[] GetInternalInventoryItemsHistory(
        int take,
        string? providerId = null,
        string? symbol = null,
        DateTime? beforeCreatedAt = null
    )
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();
        return linq2Db
            .QueryProc<InternalInventoryItemDb>(
                "[dbo].[GetInternalInventoryItemsHistory]",
                [
                    new DataParameter("@Take", take, DataType.Int32),
                    new DataParameter("@Symbol", symbol, DataType.VarChar),
                    new DataParameter("@ProviderId", providerId, DataType.VarChar),
                    new DataParameter("@BeforeCreatedAt", beforeCreatedAt, DataType.DateTime),
                ]
            )
            .ToArray();
    }

    public SymbolQuantityDb[] GetInternalInventorySymbolQuantities(
        DateTime from,
        DateTime to,
        IReadOnlyCollection<string>? includeSources = null,
        IReadOnlyCollection<string>? excludeSources = null,
        IReadOnlyCollection<InternalInventoryItem.State>? statuses = null,
        IReadOnlyCollection<CreatingType>? creatingTypes = null
    )
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();

        var parameters = new List<DataParameter>()
        {
            new DataParameter("@From", from, DataType.DateTime),
            new DataParameter("@To", to, DataType.DateTime),
        };

        parameters.AddAsJsonIfNotEmpty("@IncludeSourcesJson", includeSources);
        parameters.AddAsJsonIfNotEmpty("@ExcludeSourcesJson", excludeSources);
        parameters.AddAsJsonIfNotEmpty(
            "@StatusesJson",
            statuses?.Select(x => x.ToString()).ToArray()
        );
        parameters.AddAsJsonIfNotEmpty(
            "@CreatingTypesJson",
            creatingTypes?.Select(x => x.ToString()).ToArray()
        );

        return linq2Db
            .QueryProc<SymbolQuantityDb>(
                "[dbo].[GetInternalInventorySymbolQuantities]",
                parameters.ToArray()
            )
            .ToArray();
    }

    public void AddInternalInventoryItem(InternalInventoryItemDb inventoryItem)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var linq2Db = scope.ServiceProvider.GetRequiredService<DbConnection>();

        var parameters = new[]
        {
            new DataParameter("@Id", inventoryItem.Id),
            new DataParameter("@Version", inventoryItem.Version),
            new DataParameter("@Symbol", inventoryItem.Symbol),
            new DataParameter("@Price", inventoryItem.Price),
            new DataParameter("@Quantity", inventoryItem.Quantity),
            new DataParameter("@SoldQuantity", inventoryItem.SoldQuantity),
            new DataParameter("@Source", inventoryItem.Source),
            new DataParameter("@CreatingType", inventoryItem.CreatingType),
            new DataParameter("@Tag", inventoryItem.Tag),
            new DataParameter("@CoveredInvItemId", inventoryItem.CoveredInvItemId),
            new DataParameter("@Status", inventoryItem.Status),
            new DataParameter("@CreatedAt", SqlDbType.DateTime2)
            {
                Value = inventoryItem.CreatedAt,
            },
        };
        linq2Db.ExecuteProc("[dbo].[AddInternalInventoryItem]", parameters);
    }
}
