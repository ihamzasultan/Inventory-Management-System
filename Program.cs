using InventoryManagementSystem.Inventory.Domain;
using InventoryManagementSystem.Inventory.infrastructure;
using InventoryManagementSystem.Inventory.infrastructure.Services;
using InventoryManagementSystem.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IApplicationUsersService, ApplicationUsersService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();



var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Auto-apply migrations (optional)
    var db = services.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // ✅ Seed Roles and Admin user
    await RoleSeeder.SeedRolesAndAdminAsync(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();
