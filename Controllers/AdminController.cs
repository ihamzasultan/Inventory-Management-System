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

    public async Task<IActionResult> Dashboard(string searchTerm)
    {
         var productsQuery = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();

            productsQuery = productsQuery.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                (p.Sku != null && p.Sku.ToLower().Contains(searchTerm)));
        }

        var products = await productsQuery.ToListAsync();
        ViewBag.SearchTerm = searchTerm; // To retain value in input field
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

        // Try to find an existing product by Name (or you can use SKU if it's unique)
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
        return RedirectToAction(nameof(Dashboard));
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
