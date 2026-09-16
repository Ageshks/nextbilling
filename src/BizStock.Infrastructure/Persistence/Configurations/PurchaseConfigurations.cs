using BizStock.Domain.Purchases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BizStock.Infrastructure.Persistence.Configurations;

/// <summary>Purchase header mapping.</summary>
public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Purchase> b)
    {
        b.ToTable("Purchases");
        b.HasIndex(p => p.PurchaseNumber).IsUnique();
        b.HasIndex(p => p.DateUtc);
        b.Property(p => p.PurchaseNumber).HasMaxLength(40).IsRequired();
        b.Property(p => p.SupplierInvoiceNumber).HasMaxLength(40);
        b.Property(p => p.SupplierName).HasMaxLength(160).IsRequired();
        b.Property(p => p.Notes).HasMaxLength(1000);
        Precision(b, nameof(Purchase.SubTotal));
        Precision(b, nameof(Purchase.DiscountAmount));
        Precision(b, nameof(Purchase.TaxAmount));
        Precision(b, nameof(Purchase.CGST));
        Precision(b, nameof(Purchase.SGST));
        Precision(b, nameof(Purchase.IGST));
        Precision(b, nameof(Purchase.OtherCharges));
        Precision(b, nameof(Purchase.RoundOff));
        Precision(b, nameof(Purchase.GrandTotal));
        Precision(b, nameof(Purchase.PaidAmount));
        Precision(b, nameof(Purchase.BalanceAmount));
        b.HasMany(p => p.Items).WithOne(i => i.Purchase).HasForeignKey(i => i.PurchaseId).OnDelete(DeleteBehavior.Cascade);
    }

    private static void Precision(EntityTypeBuilder<Purchase> b, string prop) =>
        b.Property(prop).HasPrecision(18, 2);
}

/// <summary>Purchase line mapping.</summary>
public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PurchaseItem> b)
    {
        b.ToTable("PurchaseItems");
        b.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
        b.Property(i => i.BatchNumber).HasMaxLength(40);
        b.Property(i => i.Note).HasMaxLength(500);
        Precision(b, nameof(PurchaseItem.UnitPrice));
        Precision(b, nameof(PurchaseItem.DiscountAmount));
        Precision(b, nameof(PurchaseItem.TaxableAmount));
        Precision(b, nameof(PurchaseItem.TaxAmount));
        Precision(b, nameof(PurchaseItem.LineTotal));
        Precision(b, nameof(PurchaseItem.GSTRate));
        b.HasIndex(i => i.ProductId);
    }

    private static void Precision(EntityTypeBuilder<PurchaseItem> b, string prop) =>
        b.Property(prop).HasPrecision(18, 2);
}

/// <summary>Purchase return mapping.</summary>
public class PurchaseReturnConfiguration : IEntityTypeConfiguration<PurchaseReturn>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PurchaseReturn> b)
    {
        b.ToTable("PurchaseReturns");
        b.HasIndex(r => r.ReturnNumber).IsUnique();
        b.Property(r => r.ReturnNumber).HasMaxLength(40).IsRequired();
        b.Property(r => r.PurchaseNumber).HasMaxLength(40).IsRequired();
        b.Property(r => r.SupplierName).HasMaxLength(160).IsRequired();
        b.Property(r => r.Reason).HasMaxLength(500);
        b.Property(r => r.SubTotal).HasPrecision(18, 2);
        b.Property(r => r.TaxAmount).HasPrecision(18, 2);
        b.Property(r => r.TotalAmount).HasPrecision(18, 2);
        b.HasMany(r => r.Items).WithOne().HasForeignKey(i => i.PurchaseReturnId).OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Purchase return line mapping.</summary>
public class PurchaseReturnItemConfiguration : IEntityTypeConfiguration<PurchaseReturnItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PurchaseReturnItem> b)
    {
        b.ToTable("PurchaseReturnItems");
        b.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
        b.Property(i => i.UnitPrice).HasPrecision(18, 2);
        b.Property(i => i.TaxAmount).HasPrecision(18, 2);
        b.Property(i => i.LineTotal).HasPrecision(18, 2);
        b.Property(i => i.GSTRate).HasPrecision(5, 2);
    }
}
