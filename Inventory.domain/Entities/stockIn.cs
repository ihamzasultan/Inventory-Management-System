using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Inventory.Domain
{
    public class StockIn
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }

        public string Remarks { get; set; } = string.Empty;
        public DateTime StockedAt { get; set; } = DateTime.Now;
        
        // Navigation property to link to Product
        public required Product Product { get; set; }
    }
}