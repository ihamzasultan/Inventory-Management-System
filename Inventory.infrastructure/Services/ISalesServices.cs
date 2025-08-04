using InventoryManagementSystem.Inventory.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Inventory.infrastructure.Services
{
    public interface ISalesServices
    {
        // Define methods related to sales operations
        Task<StockOut> GetStockOutByIdAsync(int id);
        Task<ActionResult> AddStockAsync(StockIn stockIn);
        Task<ActionResult> AddStockOutAsync(StockOut stockOut);
        Task<IEnumerable<StockOut>> ViewSalesRecordsAsync(string searchTerm);
        Task<IEnumerable<StockIn>> ViewStockRecordsAsync(string searchTerm);
        Task<ActionResult> ReturnItemAsync(ReturnItem returnItem);
        Task<IEnumerable<ReturnItem>> ViewAllReturnsAsync(string searchTerm);
    }
}