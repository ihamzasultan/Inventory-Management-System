using InventoryManagementSystem.Inventory.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Inventory.infrastructure.Services
{
    public interface IInventoryService
    {
        // Define methods for inventory management
        Task<IEnumerable<Product>> GetAllInventoryItemsAsync(string searchTerm);
        Task<Product> GetInventoryItemByIdAsync(int id);
        Task AddInventoryItemAsync(Product product);
        Task UpdateInventoryItemAsync(Product product);
        Task DeleteInventoryItemAsync(int id);
    }
}