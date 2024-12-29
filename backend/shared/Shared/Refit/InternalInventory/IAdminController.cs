using Refit;


namespace Shared.Refit.InternalInventory;

public interface IAdminController
{
  [Get("/api/admin/GetInventory")]
  Task<IList<InternalInventoryItem>> Admin_GetInventory(string symbol, string providerId = null); 


  [Post("/api/admin/AddInventory")]
  Task<InternalInventoryItem> Admin_AddInventory([Body] AddInternalInventoryItemRequest item);


  [Post("/api/admin/InternalInventoryItem")]
  Task Admin_UpdateInventoryItem([Body] InternalInventoryItem item);


  [Post("/api/admin/MakeInactive")]
  Task Admin_MakeInactive([Body] InternalInventoryItem item);


  [Post("/api/admin/MakeActive")]
  Task Admin_MakeActive([Body] InternalInventoryItem item);


  [Post("/api/admin/DeleteInventoryItem")]
  Task Admin_DeleteInventoryItem([Body] InternalInventoryItem item);
}
