using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.Inventory.Domain
{
    public class ApplicationUser : IdentityUser
    {
       public string FullName { get; set; } = string.Empty;

    }
}