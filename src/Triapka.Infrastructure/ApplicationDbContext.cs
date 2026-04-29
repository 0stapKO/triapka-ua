using Microsoft.EntityFrameworkCore;
using Triapka.Domain.Entities;

namespace Triapka.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }

    public DbSet<ProductCategory> ProductCategories { get; set; }

    public DbSet<ProductImage> ProductImages { get; set; }

    public DbSet<Cart> Carts { get; set; }

    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<WishlistItem> WishlistItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductCategory>().HasKey(pc => pc.CategoryId);
        modelBuilder.Entity<ProductImage>().HasKey(pi => pi.ImageId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<ProductImage>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.ProductId);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId);

        modelBuilder.Entity<WishlistItem>()
            .HasOne(wi => wi.Product)
            .WithMany()
            .HasForeignKey(wi => wi.ProductId);

        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(10,2)");
        modelBuilder.Entity<Product>().Property(p => p.Rating).HasColumnType("decimal(3,2)");
    }
}