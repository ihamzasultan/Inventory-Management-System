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

    public IActionResult Dashboard()
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
    public async Task<IActionResult> ViewStockRecord()
    {
        var stockRecords = await _saleService.ViewStockRecordsAsync(string.Empty);
        return View(stockRecords);
    }
}
