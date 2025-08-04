using System.Linq.Expressions;
using System.Security.Claims;
using InventoryManagementSystem.Inventory.Domain;
using InventoryManagementSystem.Inventory.infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[Authorize(Roles = "Salesperson")]
public class SalesPersonController : Controller
{
    public readonly IInventoryService _inventoryService;
    public readonly ISalesServices _saleService;
    private readonly ILogger<SalesPersonController> _logger;

    public SalesPersonController(IInventoryService inventoryService, ISalesServices salesService, ILogger<SalesPersonController> logger)
    {
        _inventoryService = inventoryService;
        _saleService = salesService;
        _logger = logger;
    }

    public IActionResult StockOut()
    {
        return View();
    }

    public async Task<IActionResult> ViewInventory()
    {
        var productlist = await _inventoryService.GetAllInventoryItemsAsync(string.Empty);
        return View(productlist);
    }

    public async Task<IActionResult> AddStock(StockIn stockIn)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _logger.LogInformation("Adding stock for product: {ProductSku}", stockIn.Product?.Sku);
                var result = await _saleService.AddStockAsync(stockIn);
                if (result is OkResult)
                {
                    return RedirectToAction("ViewInventory");
                }
                ModelState.AddModelError(string.Empty, "Failed to add stock.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding stock.");
                ModelState.AddModelError(string.Empty, "An error occurred while adding stock.");
            }

        }
        else
        {
            _logger.LogError("Model state is invalid for adding stock.");
        }
        return View(stockIn);
    }
    [HttpGet]
    public async Task<IActionResult> ViewStockRecord(string searchTerm)
    {
        var stockRecords = await _saleService.ViewStockRecordsAsync(searchTerm);
        return View(stockRecords);
    }

    [HttpPost]
    public async Task<IActionResult> StockOut(StockOut stockOut)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            ModelState.AddModelError(string.Empty, "Unable to determine current user.");
            return View(stockOut);
        }
        stockOut.SalespersonId = userId;
        _logger.LogInformation($"Processing stock out for product: {stockOut.Product?.Sku}");
        _logger.LogInformation($"Processing stock out for UserId: {stockOut.SalespersonId}");

        if (ModelState.IsValid)
        {
            try
            {
                _logger.LogInformation("Processing stock out for product: {ProductSku}", stockOut.Product?.Sku);
                var result = await _saleService.AddStockOutAsync(stockOut);
                if (result is OkResult)
                {
                    return RedirectToAction("ViewInventory");
                }
                ModelState.AddModelError(string.Empty, "Failed to process stock out.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing stock out.");
                ModelState.AddModelError(string.Empty, "An error occurred while processing stock out.");
            }

        }
        else
        {
            _logger.LogError("Model state is invalid for processing stock out.");
        }

        return View(stockOut);

    }

    [HttpGet]
    public async Task<IActionResult> ViewSalesRecord(string searchTerm)
    {
        var salesRecords = await _saleService.ViewSalesRecordsAsync(searchTerm);
        return View(salesRecords);
    }

    public async Task<IActionResult> ReturnSaleItem(int id)
    {
        var stockOut = await _saleService.GetStockOutByIdAsync(id);
        if (stockOut == null)
        {
            return NotFound();
        }

        var returnItem = new ReturnItem
        {
            StockOutId = stockOut.Id,
            DeliveredQuantity = stockOut.Quantity,
            StockOut = stockOut
        };
        _logger.LogInformation("Returning sale item for StockOutId: {StockOutId}", returnItem.StockOutId);
        _logger.LogInformation("REturn ID before pressing return: " + returnItem.Id);
        return View(returnItem);
    }

    [HttpPost]
    public async Task<IActionResult> ReturnSaleItem(ReturnItem returnItem)
    {

        try
        {
            Console.WriteLine("Returning Item Name: " + returnItem.StockOut?.Product?.Name);
            Console.WriteLine("Returning Item Quantity: " + returnItem.ReturnedQuantity);
            Console.WriteLine("Returning Item ID: " + returnItem.Id);

            _logger.LogInformation("Returning sale item for StockOutId: {StockOutId}", returnItem.StockOutId);
            var result = await _saleService.ReturnItemAsync(returnItem);
            if (result is OkResult)
            {
                TempData["SuccessMessage"] = "Item returned successfully!";
                return RedirectToAction("ViewSalesRecord");
            }
            else
            {
                _logger.LogError("Failed to return sale item for StockOutId: {StockOutId}", returnItem.StockOutId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error returning sale item for StockOutId: {StockOutId}", returnItem.StockOutId);
        }
        return View(returnItem);
    }

    [HttpGet]
    public async Task<IActionResult> ViewAllReturns(string searchTerm)
    {
        var returnItems = await _saleService.ViewAllReturnsAsync(searchTerm);
        return View(returnItems);
    }
       
        
}


