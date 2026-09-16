using BizStock.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BizStock.Infrastructure.Persistence.Configurations;

/// <summary>Product mapping: unique indexes and decimal precision.</summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products");
        b.HasIndex(p => p.ProductCode).IsUnique();
        b.HasIndex(p => p.SKU).IsUnique().HasFilter("[SKU] IS NOT NULL");
        b.HasIndex(p => p.Barcode).IsUnique().HasFilter("[Barcode] IS NOT NULL");
        b.Property(p => p.Name).HasMaxLength(200).IsRequired();
        b.Property(p => p.ProductCode).HasMaxLength(30).IsRequired();
        b.Property(p => p.SKU).HasMaxLength(60);
        b.Property(p => p.Barcode).HasMaxLength(60);
        b.Property(p => p.HSNCode).HasMaxLength(20);
        b.Property(p => p.PurchasePrice).HasPrecision(18, 2);
        b.Property(p => p.SellingPrice).HasPrecision(18, 2);
        b.Property(p => p.WholesalePrice).HasPrecision(18, 2);
        b.Property(p => p.MRP).HasPrecision(18, 2);
        b.Property(p => p.GSTRate).HasPrecision(5, 2);
        b.Property(p => p.MinimumStock).HasPrecision(18, 3);
        b.HasOne(p => p.Category).WithMany().OnDelete(DeleteBehavior.SetNull);
        b.HasOne(p => p.Brand).WithMany().OnDelete(DeleteBehavior.SetNull);
        b.HasOne(p => p.Unit).WithMany().OnDelete(DeleteBehavior.SetNull);
        b.HasOne(p => p.PreferredSupplier).WithMany().OnDelete(DeleteBehavior.SetNull);
        b.HasOne(p => p.StockSummary).WithOne(s => s.Product)
            .HasForeignKey<Domain.Inventory.StockSummary>(s => s.ProductId);
        b.HasQueryFilter(p => p.DeletedAtUtc == null);
    }
}

/// <summary>Category mapping.</summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.ToTable("Categories");
        b.HasIndex(c => c.Name).IsUnique();
        b.Property(c => c.Name).HasMaxLength(120).IsRequired();
    }
}

