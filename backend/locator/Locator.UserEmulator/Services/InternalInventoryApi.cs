using Locator.UserEmulator.Options;
using Refit;
using Shared;
using Shared.Refit;

namespace Locator.UserEmulator.Services;

public class InternalInventoryApi
{
    private readonly RandomHelper _randomHelper;
    private readonly IInternalInventoryApi internalInventoryApi;

    public InternalInventoryApi(ApiOptions apiOptions, RandomHelper randomHelper)
    {
        _randomHelper = randomHelper;
        internalInventoryApi = RestService.For<IInternalInventoryApi>(
            apiOptions.InternalInventoryUrl
        );
    }

    public void InitInternalInventory()
    {
        var symbols = _randomHelper.GetSymbols();

        var tasks = symbols.Select(symbol =>
            AddInventory(
                new AddInternalInventoryItemRequest
                {
                    Symbol = symbol,
                    Quantity = 999_999_999,
                    Price = _randomHelper.GetRandomMockPrice(),
                    Source = _randomHelper.GetRandomSource(),
                    CreatingType = CreatingType.SingleEntry,
                }
            )
        );

        Task.WaitAll(tasks);
    }

    public Task<InternalInventoryItem> AddInventory(AddInternalInventoryItemRequest item)
    {
        return internalInventoryApi.Admin_AddInventory(item);
    }
}
