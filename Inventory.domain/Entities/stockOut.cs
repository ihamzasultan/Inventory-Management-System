    using System.ComponentModel.DataAnnotations;

    namespace InventoryManagementSystem.Inventory.Domain
    {
        public class StockOut
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }

            public required string Client { get; set; }

            public required string HandOverTo { get; set; }
            public string? SalespersonId { get; set; }

            // ✅ Navigation property to Identity user
            public ApplicationUser? Salesperson { get; set; }
            public DateTime Date { get; set; } = DateTime.Now;
            // Navigation property to link to Product
            public Product? Product { get; set; }
        }
    }