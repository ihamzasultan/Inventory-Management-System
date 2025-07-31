using System.ComponentModel.DataAnnotations;
using InventoryManagementSystem.Inventory.Domain;

public class ReturnItem
{
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }
    [Required]
    public int Quantity { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public DateTime ReturnedAt { get; set; } = DateTime.Now;

    public required Product Product { get; set; }
}