using InventoryManagementSystem.Inventory.Domain;
using InventoryManagementSystem.Inventory.infrastructure;
using InventoryManagementSystem.Inventory.infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public readonly IInventoryService _inventoryService;

    public AdminController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<IActionResult> Dashboard(string searchTerm)
    {
        var products = await _inventoryService.GetAllInventoryItemsAsync(searchTerm); 
        return View(products);
    }

    public IActionResult AddInventory()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddInventory(Product product)
    {
        if (!ModelState.IsValid)
            return View(product);
        await _inventoryService.AddInventoryItemAsync(product);        
        return RedirectToAction(nameof(Dashboard));
    }
    public async Task<IActionResult> EditInventory(int id)
    {
        var product = await _inventoryService.GetInventoryItemByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product); // /Views/Admin/EditInventory.cshtml
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditInventory(Product product)
    {
        if (ModelState.IsValid)
        {
            await _inventoryService.UpdateInventoryItemAsync(product);
            return RedirectToAction(nameof(Dashboard));
        }
        return View(product); // /Views/Admin/EditInventory.cshtml
    }

    
    public async Task<IActionResult> DeleteInventory(int id)
    {
        await _inventoryService.DeleteInventoryItemAsync(id);
        return RedirectToAction(nameof(Dashboard));
    }

}
