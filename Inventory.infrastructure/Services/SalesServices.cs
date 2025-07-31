using InventoryManagementSystem.Inventory.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Inventory.infrastructure.Services
{
    public class SalesServices : ISalesServices
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SalesServices> _logger;
        private readonly IInventoryService _inventoryService;

        public SalesServices(AppDbContext context, ILogger<SalesServices> logger, IInventoryService inventoryService)
        {
            _context = context;
            _logger = logger;
            _inventoryService = inventoryService;
        }

        public async Task<ActionResult> AddStockAsync(StockIn stockIn)
        {
            try
            {


                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Sku.ToLower() == stockIn.Product.Sku.ToLower());

                if (existingProduct == null)        // Check if the product exists 
                {                                  // If not, create a new product entry
                    Product newProduct = new Product
                    {
                        Name = stockIn.Product.Name,
                        Sku = stockIn.Product.Sku,
                        Quantity = stockIn.Quantity,
                        CreatedAt = DateTime.Now
                    };
                    _context.Products.Add(newProduct);
                    await _context.SaveChangesAsync();

                    existingProduct = newProduct;

                }
                else
                {
                    // If the product exists, update the quantity
                    existingProduct.Quantity += stockIn.Quantity;
                }

                stockIn.ProductId = existingProduct.Id;
                stockIn.Product = null;
                _context.StockIns.Add(stockIn);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding stock for product: {ProductSku}", stockIn.Product?.Sku);
                return new BadRequestObjectResult("An error occurred while adding stock.");
            }

            return new OkResult();
        }

        public Task<ActionResult> ReturnItemAsync(ReturnItem returnItem)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult> ViewAllReturnsAsync(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult> ViewSalesRecordsAsync(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<StockIn>> ViewStockRecordsAsync(string searchTerm)
        {
            var productsQuery = _context.StockIns
                                        .Include(s => s.Product)        
                                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                productsQuery = productsQuery.Where(s => s.Product.Name.Contains(searchTerm) || s.Product.Sku.Contains(searchTerm));
            }

            var stockRecords = await productsQuery.ToListAsync();
            return stockRecords;
        }
    }
}