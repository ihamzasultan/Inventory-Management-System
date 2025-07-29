using InventoryManagementSystem.Inventory.Domain;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.Inventory.infrastructure
{
    public interface IApplicationUsersService
    {
        // Define methods for user management
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();

        Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
        Task DeleteUserAsync(string userId);

        Task<(bool Success, IList<string> Roles)> LoginAsyncWithRoles(string email, string password);

        Task LogoutAsync();
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);


    }
}