using Microsoft.AspNetCore.Identity;
using InventoryManagementSystem.Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Models;
using System.Security.Claims;

namespace InventoryManagementSystem.Inventory.infrastructure
{
    public class ApplicationUsersService : IApplicationUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ApplicationUsersService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }


        public async Task<ApplicationUser?> UpdateUserAsync(ApplicationUser user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser == null) return null;

            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.UserName = user.Email;

            var result = await _userManager.UpdateAsync(existingUser);
            return result.Succeeded ? existingUser : null;
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                await _userManager.DeleteAsync(user);
        }

        public async Task<(bool Success, IList<string> Roles)> LoginAsyncWithRoles(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (!result.Succeeded)
                return (false, new List<string>());

            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);

            return (true, roles);
        }


        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Salesperson");
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return result;
        }

       public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        
        public string GetCurrentUserId(ClaimsPrincipal user)
        {
            return _userManager.GetUserId(user);
        }


    }
}
