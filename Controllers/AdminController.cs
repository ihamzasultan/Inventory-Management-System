using InventoryManagementSystem.Inventory.Domain;
using InventoryManagementSystem.Inventory.infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        // Fetch data for the admin dashboard
        var items = await _context.Products.ToListAsync();
        return View(items); // /Views/Admin/Index.cshtml
    }

    public IActionResult AddInventory()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddInventory(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }
        return View(product); // /Views/Admin/AddInventory.cshtml
    }

    public async Task<IActionResult> EditInventory(int id)
    {
        var product = await _context.Products.FindAsync(id);
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
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }
        return View(product); // /Views/Admin/EditInventory.cshtml
    }

    
    public async Task<IActionResult> DeleteInventory(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Dashboard));
    }

}
