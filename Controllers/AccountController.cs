using InventoryManagementSystem.Inventory.infrastructure;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    public class AccountController : Controller
    {

        private readonly IApplicationUsersService _userService;

        public AccountController(IApplicationUsersService userService)
        {
            _userService = userService;
        }

      [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, roles) = await _userService.LoginAsyncWithRoles(model.Email, model.Password);

            if (success)
            {
                if (roles.Contains("Admin"))
                    return RedirectToAction("Dashboard", "Admin");

                if (roles.Contains("Salesperson"))
                    return RedirectToAction("Dashboard", "SalesPerson");

                return RedirectToAction("Index", "Home"); // fallback
            }

            ModelState.AddModelError("", "Invalid login credentials");
            return View(model);
        }


        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userService.RegisterAsync(model);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            Console.WriteLine("Model posted");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid");
                return View(model);
            }

            var userId = _userService.GetCurrentUserId(User);
            Console.WriteLine("User ID: " + userId);

            var result = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                Console.WriteLine("Password changed successfully");
                TempData["SuccessMessage"] = "Password updated successfully!";
                await _userService.LogoutAsync();
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                Console.WriteLine("Error: " + error.Description);
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }



        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Login");
        }
    }
}