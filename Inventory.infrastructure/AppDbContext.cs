namespace InventoryManagementSystem.Inventory.infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using InventoryManagementSystem.Inventory.Domain;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<StockIn> StockIns { get; set; }
        public DbSet<StockOut> StockOuts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ReturnItem> ReturnItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<StockIn>().ToTable("StockIns");
            modelBuilder.Entity<StockOut>().ToTable("StockOuts");
            modelBuilder.Entity<ApplicationUser>().ToTable("ApplicationUsers");
            modelBuilder.Entity<ReturnItem>().ToTable("ReturnItems");

            base.OnModelCreating(modelBuilder);
            // Additional configurations can be added here
        }
    }
}