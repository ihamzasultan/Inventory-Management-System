using InventoryManagementSystem.Inventory.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Inventory.infrastructure.Services
{
    public interface ISalesServices
    {
        // Define methods related to sales operations
        Task<ActionResult> AddStockAsync(StockIn stockIn);
        Task<ActionResult> ViewSalesRecordsAsync(string searchTerm);
        Task<IEnumerable<StockIn>> ViewStockRecordsAsync(string searchTerm);
        Task<ActionResult> ReturnItemAsync(ReturnItem returnItem);
        Task<ActionResult> ViewAllReturnsAsync(string searchTerm);
    }
}