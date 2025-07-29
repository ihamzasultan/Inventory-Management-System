using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Inventory.Domain
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public required string Sku { get; set; }

        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}   