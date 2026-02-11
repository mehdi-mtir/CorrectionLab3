using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Product>(entity =>
        {
            // 1. Nom de la table
            entity.ToTable("Products");
            
            // 2. Configuration de la clé primaire
            entity.HasKey(p => p.Id);
            
            // 3. Configuration des propriétés obligatoires
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(2000);
                
            entity.Property(p => p.Category)
                .IsRequired()
                .HasMaxLength(50);
            
            // 4. Propriétés optionnelles
            entity.Property(p => p.ImageUrl)
                .HasMaxLength(500);
            
            // 5. Précision décimale pour Price
            entity.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            // 6. Configuration des index
            entity.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Products_Name");
                
            entity.HasIndex(p => p.Category)
                .HasDatabaseName("IX_Products_Category");
            
            // 7. Données de seed (au moins 5 produits)
            entity.HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop Dell XPS 15",
                    Description = "Ordinateur portable haute performance avec écran 15 pouces, processeur Intel i7, 16GB RAM",
                    Price = 1499.99m,
                    StockQuantity = 25,
                    Category = "Electronics",
                    ImageUrl = "https://example.com/images/dell-xps-15.jpg",
                    CreatedDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 2,
                    Name = "iPhone 15 Pro",
                    Description = "Smartphone Apple dernière génération avec puce A17 Pro, caméra 48MP",
                    Price = 1199.99m,
                    StockQuantity = 50,
                    Category = "Electronics",
                    ImageUrl = "https://example.com/images/iphone-15-pro.jpg",
                    CreatedDate = new DateTime(2024, 1, 20, 14, 30, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 3,
                    Name = "Samsung 65\" QLED TV",
                    Description = "Téléviseur QLED 4K 65 pouces avec HDR, Smart TV et son Dolby Atmos",
                    Price = 1899.99m,
                    StockQuantity = 15,
                    Category = "Electronics",
                    ImageUrl = "https://example.com/images/samsung-qled-65.jpg",
                    CreatedDate = new DateTime(2024, 2, 1, 9, 15, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 4,
                    Name = "Sony WH-1000XM5",
                    Description = "Casque audio sans fil avec réduction de bruit active de pointe",
                    Price = 399.99m,
                    StockQuantity = 40,
                    Category = "Audio",
                    ImageUrl = "https://example.com/images/sony-wh1000xm5.jpg",
                    CreatedDate = new DateTime(2024, 2, 5, 11, 45, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 5,
                    Name = "iPad Pro 12.9\"",
                    Description = "Tablette Apple Pro avec puce M2, écran Liquid Retina XDR, 256GB",
                    Price = 1099.99m,
                    StockQuantity = 30,
                    Category = "Electronics",
                    ImageUrl = "https://example.com/images/ipad-pro-12.jpg",
                    CreatedDate = new DateTime(2024, 2, 10, 16, 20, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 6,
                    Name = "Logitech MX Master 3S",
                    Description = "Souris sans fil ergonomique pour professionnels avec 8000 DPI",
                    Price = 99.99m,
                    StockQuantity = 75,
                    Category = "Accessories",
                    ImageUrl = "https://example.com/images/mx-master-3s.jpg",
                    CreatedDate = new DateTime(2024, 2, 12, 13, 10, 0, DateTimeKind.Utc)
                }
            );
        });
    }
}
