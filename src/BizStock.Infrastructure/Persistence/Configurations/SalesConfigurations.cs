using BizStock.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BizStock.Infrastructure.Persistence.Configurations;

/// <summary>Sale header mapping.</summary>
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Sale> b)
    {
        b.ToTable("Sales");
        b.HasIndex(s => s.InvoiceNumber).IsUnique();
        b.HasIndex(s => s.DateUtc);
        b.HasIndex(s => new { s.CustomerId, s.DateUtc });
        b.Property(s => s.InvoiceNumber).HasMaxLength(40).IsRequired();
        b.Property(s => s.CustomerName).HasMaxLength(160).IsRequired();
        b.Property(s => s.CustomerGSTIN).HasMaxLength(20);
        b.Property(s => s.CustomerStateCode).HasMaxLength(4);
        b.Property(s => s.BusinessStateCode).HasMaxLength(4);
        b.Property(s => s.SalesPerson).HasMaxLength(60);
        b.Property(s => s.Notes).HasMaxLength(1000);
        MoneyPrecision(b);
        b.HasMany(s => s.Items).WithOne().HasForeignKey(i => i.SaleId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(s => s.Payments).WithOne().HasForeignKey(p => p.SaleId).OnDelete(DeleteBehavior.Cascade);
    }

    internal static void MoneyPrecision(EntityTypeBuilder b) { }
}

/// <summary>Sale line mapping.</summary>
public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SaleItem> b)
    {
        b.ToTable("SaleItems");
        b.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
        b.Property(i => i.BatchNumber).HasMaxLength(40);
        Decimal(b, nameof(SaleItem.UnitPrice));
        Decimal(b, nameof(SaleItem.DiscountAmount));
        Decimal(b, nameof(SaleItem.TaxableAmount));
        Decimal(b, nameof(SaleItem.TaxAmount));
        Decimal(b, nameof(SaleItem.LineTotal));
        Decimal(b, nameof(SaleItem.CostPrice));
        Decimal(b, nameof(SaleItem.GSTRate));
        b.HasIndex(i => i.ProductId);
    }

    private static void Decimal(EntityTypeBuilder<SaleItem> b, string prop) =>
        b.Property(prop).HasPrecision(18, 2);
}

/// <summary>Sale payment mapping.</summary>
public class SalePaymentConfiguration : IEntityTypeConfiguration<SalePayment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SalePayment> b)
    {
        b.ToTable("SalePayments");
        b.Property(p => p.Amount).HasPrecision(18, 2);
        b.Property(p => p.Reference).HasMaxLength(100);
        b.Property(p => p.Note).HasMaxLength(500);
    }
}

/// <summary>Sales return mapping.</summary>
public class SalesReturnConfiguration : IEntityTypeConfiguration<SalesReturn>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SalesReturn> b)
    {
        b.ToTable("SalesReturns");
        b.HasIndex(r => r.ReturnNumber).IsUnique();
        b.Property(r => r.ReturnNumber).HasMaxLength(40).IsRequired();
        b.Property(r => r.InvoiceNumber).HasMaxLength(40).IsRequired();
        b.Property(r => r.CustomerName).HasMaxLength(160).IsRequired();
        b.Property(r => r.Reason).HasMaxLength(500);
        b.Property(r => r.SubTotal).HasPrecision(18, 2);
        b.Property(r => r.TaxAmount).HasPrecision(18, 2);
        b.Property(r => r.TotalAmount).HasPrecision(18, 2);
        b.HasMany(r => r.Items).WithOne().HasForeignKey(i => i.SalesReturnId).OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Sales return line mapping.</summary>
public class SalesReturnItemConfiguration : IEntityTypeConfiguration<SalesReturnItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SalesReturnItem> b)
    {
        b.ToTable("SalesReturnItems");
        b.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
        b.Property(i => i.UnitPrice).HasPrecision(18, 2);
        b.Property(i => i.TaxAmount).HasPrecision(18, 2);
        b.Property(i => i.LineTotal).HasPrecision(18, 2);
        b.Property(i => i.GSTRate).HasPrecision(5, 2);
    }
}
