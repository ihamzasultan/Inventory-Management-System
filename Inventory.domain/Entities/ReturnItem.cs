using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InventoryManagementSystem.Inventory.Domain;

namespace InventoryManagementSystem.Inventory.Domain;
public class ReturnItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int DeliveredQuantity { get; set; }
    public int ReturnedQuantity { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public DateTime ReturnedAt { get; set; } = DateTime.Now;
    [ForeignKey(nameof(StockOutId))]
    public StockOut? StockOut { get; set; }
    public required int StockOutId { get; set; }
}