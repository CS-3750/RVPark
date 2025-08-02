using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using RVPark.Application;
using RVPark.Core.Interfaces;
using RVPark.Core.Models;
using RVPark.Core.Utilities;
using Amazon.S3;
using Amazon;

var builder = WebApplication.CreateBuilder(args);

// Get a connection string named "DefaultConnection" from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

Console.WriteLine($"Using connection string: {connectionString}");

// Register DbContext with a localdb connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// DB Initializer
builder.Services.AddScoped<DBInitializer>();
builder.Services.AddSingleton<IEmailSender, FakeEmailSender>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<IProjectService, ProjectService>();

// Configure AWS S3
var awsAccessKey = builder.Configuration["AWS:AccessKey"];
var awsSecretKey = builder.Configuration["AWS:SecretKey"];
var awsRegion = RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"] ?? "us-east-1");

builder.Services.AddSingleton<IAmazonS3>(provider =>
{
    var config = new AmazonS3Config
    {
        RegionEndpoint = awsRegion,
        ForcePathStyle = false
    };
    return new AmazonS3Client(awsAccessKey, awsSecretKey, config);
});

builder.Services.AddScoped<IS3Service, S3Service>();

// Add Identity services with ApplicationDbContext
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add Razor Pages and API Controllers support
builder.Services.AddRazorPages();
builder.Services.AddControllers();

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
app.UseAuthorization(); // Add authorization middleware for [Authorize] attributes

app.UseSession();

app.MapRazorPages();
app.MapControllers();

await SeedDatabaseAsync(app);

app.Run();

static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DBInitializer>();
    await dbInitializer.InitializeAsync();
}
