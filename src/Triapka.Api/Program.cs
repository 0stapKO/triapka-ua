using Microsoft.EntityFrameworkCore;
using Serilog;

using Triapka.Application.Interfaces;
using Triapka.Application.Services;
using Triapka.Infrastructure;
using Triapka.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

var app = builder.Build();
Log.Information("Starting Triapka API");

app.Lifetime.ApplicationStarted.Register(() =>
    Log.Information("API started {@StartupDetails}", new
    {
        Environment = app.Environment.EnvironmentName,
        app.Environment.ApplicationName,
        app.Environment.ContentRootPath,
    }));

app.Lifetime.ApplicationStopping.Register(() =>
    Log.Information("API stopping {@ShutdownDetails}", new
    {
        Environment = app.Environment.EnvironmentName,
        app.Environment.ApplicationName,
    }));

app.Lifetime.ApplicationStopped.Register(() =>
    Log.Information("API stopped {@ShutdownDetails}", new
    {
        Environment = app.Environment.EnvironmentName,
        app.Environment.ApplicationName,
    }));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");
app.MapControllers();

app.Run();