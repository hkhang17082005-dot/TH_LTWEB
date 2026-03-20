using Lab_3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Lab_3.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Điện thoại" },
            new Category { Id = 2, Name = "Laptop" },
            new Category { Id = 3, Name = "Phụ kiện" }
        );

        modelBuilder.Entity<Product>().HasData(
        new Product 
        { 
            Id = 1, 
            Name = "iPhone 15 Pro", 
            Price = 1200.00m, 
            Description = "Flagship mới nhất từ Apple", 
              CategoryId = 1,
            ImageURL = "/images/iphone15.jpg"
        },
        new Product 
        { 
            Id = 2, 
            Name = "MacBook Pro M3", 
            Price = 2500.00m, 
            Description = "Laptop hiệu năng cao cho đồ họa", 
            CategoryId = 2,
            ImageURL = "/images/macbook.jpg"
        });
    }
}