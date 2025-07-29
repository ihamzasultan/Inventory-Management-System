using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Salesperson")]
public class StockOutController : Controller
{
    public IActionResult Create()
    {
        return View(); // /Views/StockOut/Create.cshtml
    }
}
