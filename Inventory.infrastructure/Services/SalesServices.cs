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

        public Task<ActionResult> AddStockOutAsync(StockOut stockOut)
        {
            try
            {
                var existingProduct = _context.Products.FirstOrDefault(p => p.Sku.ToLower() == stockOut.Product.Sku.ToLower());
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with SKU {ProductSku} not found for stock out.", stockOut.Product.Sku);
                    return Task.FromResult<ActionResult>(new NotFoundObjectResult("Product not found."));
                }
                else if (existingProduct.Quantity < stockOut.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for product {ProductSku}. Available: {AvailableQuantity}, Requested: {RequestedQuantity}",
                        stockOut.Product.Sku, existingProduct.Quantity, stockOut.Quantity);
                    return Task.FromResult<ActionResult>(new BadRequestObjectResult("Insufficient stock available."));
                }
                else
                {
                    existingProduct.Quantity -= stockOut.Quantity;
                    stockOut.ProductId = existingProduct.Id;
                    stockOut.Product = null;
                    _context.StockOuts.Add(stockOut);
                    _context.Products.Update(existingProduct);
                    _context.SaveChanges();
                    _logger.LogInformation("Stock out processed for product: {ProductSku}, Quantity: {Quantity}", stockOut.Product.Sku, stockOut.Quantity);
                    return Task.FromResult<ActionResult>(new OkResult());
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing stock out for product: {ProductSku}", stockOut.Product?.Sku);
                return Task.FromResult<ActionResult>(new BadRequestObjectResult("An error occurred while processing stock out."));
            }

        }

        public async Task<StockOut> GetStockOutByIdAsync(int id)
        {
            var stockOut = await _context.StockOuts.FindAsync(id);
            if (stockOut == null)
            {
                return null;
            }
            return stockOut;
        }


        public async Task<ActionResult> ReturnItemAsync(ReturnItem returnItem)
        {
            var stockOut = await _context.StockOuts.FindAsync(returnItem.StockOutId);
            Console.WriteLine($"StockOutId: {returnItem.StockOutId}, StockOut: {stockOut?.Product?.Name}");
            
            if (stockOut == null)
            {
                return new NotFoundObjectResult("Stock out record not found");
            }
            
            if (returnItem.ReturnedQuantity > stockOut.Quantity)
            {
                return new BadRequestObjectResult("Returned quantity exceeds sold quantity");
            }

            // Create a new ReturnItem instance to avoid EF tracking issues
            var newReturnItem = new ReturnItem
            {
                StockOutId = returnItem.StockOutId,
                ReturnedQuantity = returnItem.ReturnedQuantity,
                Remarks = returnItem.Remarks ?? string.Empty,
                DeliveredQuantity = stockOut.Quantity,
                ReturnedAt = DateTime.Now
            };

            // Update stock quantity
            stockOut.Quantity -= returnItem.ReturnedQuantity;
            _context.StockOuts.Update(stockOut);

            // Update the product quantity
            var product = await _context.Products.FindAsync(stockOut.ProductId);
            product.Quantity += returnItem.ReturnedQuantity;
            _context.Products.Update(product);

            // Add the new return item
            _context.ReturnItems.Add(newReturnItem);

            await _context.SaveChangesAsync(); // Use async version

            Console.WriteLine($"New ReturnItem ID: {newReturnItem.Id}");
            
            return new OkResult();
        }

        public async Task<IEnumerable<ReturnItem>> ViewAllReturnsAsync(string searchTerm)
        {
            var returnsQuery = _context.ReturnItems
                                            .Include(r => r.StockOut)
                                            .ThenInclude(s => s.Product)
                                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                returnsQuery = returnsQuery.Where(r => r.StockOut.Product.Name.Contains(searchTerm) || r.StockOut.Product.Sku.Contains(searchTerm) || r.Remarks.Contains(searchTerm)
                                                    || r.StockOut.Client.Contains(searchTerm) || r.StockOut.HandOverTo.Contains(searchTerm));
            }

            return await returnsQuery.ToListAsync();
        }

        public async Task<IEnumerable<StockOut>> ViewSalesRecordsAsync(string searchTerm)
        {
            var salesQuery = _context.StockOuts.
                                            Include(s => s.Product)
                                            .Include(s => s.Salesperson)
                                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                salesQuery = salesQuery.Where(s => s.Product.Name.Contains(searchTerm) || s.Product.Sku.Contains(searchTerm) || s.Client.Contains(searchTerm));
            }

            var salesRecords = await salesQuery.ToListAsync();
            return salesRecords;
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