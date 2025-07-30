using InventoryManagementSystem.Inventory.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Inventory.infrastructure.Services
{
    public class InventoryService : IInventoryService
    {
        // Implement the methods defined in the interface
        private readonly AppDbContext _context;
        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllInventoryItemsAsync(string searchTerm)
        {
            // Implementation
            // Placeholder for actual implementation
            var productsQuery = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();

                productsQuery = productsQuery.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    (p.Sku != null && p.Sku.ToLower().Contains(searchTerm)));
            }

            var products = await productsQuery.ToListAsync();
            return products;
            
        }


        public async Task<Product> GetInventoryItemByIdAsync(int id)
        {
            // Implementation
            return await _context.Products.FindAsync(id) ?? throw new KeyNotFoundException("Product not found");
        }

        public async Task AddInventoryItemAsync(Product product)
        {
            // Implementation
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Sku.ToLower() == product.Sku.ToLower());

            if (existingProduct != null)
            {
                // Product exists, just update quantity
                existingProduct.Quantity += product.Quantity;
                _context.Products.Update(existingProduct);
            }
            else
            {
                // New product, insert as new
                product.CreatedAt = DateTime.Now;
                _context.Products.Add(product);

            }
           
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInventoryItemAsync(Product product)
        {
            // Implementation
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInventoryItemAsync(int id)
        {
            // Implementation
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found");
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

    }
}