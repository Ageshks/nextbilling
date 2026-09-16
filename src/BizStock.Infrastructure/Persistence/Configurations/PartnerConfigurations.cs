using BizStock.Domain.Partners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BizStock.Infrastructure.Persistence.Configurations;

/// <summary>Customer mapping.</summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Customer> b)
    {
        b.ToTable("Customers");
        b.HasIndex(c => c.CustomerCode).IsUnique();
        b.HasIndex(c => c.Phone);
        b.Property(c => c.CustomerCode).HasMaxLength(30).IsRequired();
        b.Property(c => c.Name).HasMaxLength(160).IsRequired();
        b.Property(c => c.GSTIN).HasMaxLength(20);
        b.Property(c => c.StateCode).HasMaxLength(4);
        b.Property(c => c.Phone).HasMaxLength(20);
        b.Property(c => c.AlternatePhone).HasMaxLength(20);
        b.Property(c => c.Email).HasMaxLength(160);
        b.Property(c => c.OutstandingBalance).HasPrecision(18, 2);
        b.Property(c => c.CreditLimit).HasPrecision(18, 2);
        b.Property(c => c.Notes).HasMaxLength(1000);
    }
}

/// <summary>Supplier mapping.</summary>
public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Supplier> b)
    {
        b.ToTable("Suppliers");
        b.HasIndex(s => s.SupplierCode).IsUnique();
        b.Property(s => s.SupplierCode).HasMaxLength(30).IsRequired();
        b.Property(s => s.Name).HasMaxLength(160).IsRequired();
        b.Property(s => s.GSTIN).HasMaxLength(20);
        b.Property(s => s.StateCode).HasMaxLength(4);
        b.Property(s => s.OutstandingBalance).HasPrecision(18, 2);
        b.Property(s => s.Notes).HasMaxLength(1000);
    }
}

/// <summary>Customer ledger mapping.</summary>
public class CustomerLedgerConfiguration : IEntityTypeConfiguration<CustomerLedger>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CustomerLedger> b)
    {
        b.ToTable("CustomerLedgers");
        b.HasIndex(l => new { l.CustomerId, l.DateUtc });
        b.HasIndex(l => l.Reference);
        b.Property(l => l.Reference).HasMaxLength(40).IsRequired();
        b.Property(l => l.Debit).HasPrecision(18, 2);
        b.Property(l => l.Credit).HasPrecision(18, 2);
        b.Property(l => l.Description).HasMaxLength(500);
        b.HasOne(l => l.Customer).WithMany().OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Supplier ledger mapping.</summary>
public class SupplierLedgerConfiguration : IEntityTypeConfiguration<SupplierLedger>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SupplierLedger> b)
    {
        b.ToTable("SupplierLedgers");
        b.HasIndex(l => new { l.SupplierId, l.DateUtc });
        b.HasIndex(l => l.Reference);
        b.Property(l => l.Reference).HasMaxLength(40).IsRequired();
        b.Property(l => l.Debit).HasPrecision(18, 2);
        b.Property(l => l.Credit).HasPrecision(18, 2);
        b.Property(l => l.Description).HasMaxLength(500);
        b.HasOne(l => l.Supplier).WithMany().OnDelete(DeleteBehavior.Cascade);
    }
}
