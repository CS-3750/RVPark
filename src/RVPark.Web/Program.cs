using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RVPark.Application;

var builder = WebApplication.CreateBuilder(args);

// Get connection string named "DefaultConnection" from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

Console.WriteLine($"Using connection string: {connectionString}");

// Register DbContext with localdb connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity services with ApplicationDbContext
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add Razor Pages support (if you need MVC, add services.AddControllersWithViews() instead)
builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Make sure to add authentication middleware
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
