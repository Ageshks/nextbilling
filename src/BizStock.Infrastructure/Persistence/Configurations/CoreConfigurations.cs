using BizStock.Domain.Auditing;
using BizStock.Domain.Business;
using BizStock.Domain.Expenses;
using BizStock.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BizStock.Infrastructure.Persistence.Configurations;

/// <summary>Business profile mapping.</summary>
public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Business> b)
    {
        b.ToTable("Businesses");
        b.Property(x => x.Name).HasMaxLength(160).IsRequired();
        b.Property(x => x.LegalName).HasMaxLength(160);
        b.Property(x => x.GSTIN).HasMaxLength(20);
        b.Property(x => x.PAN).HasMaxLength(12);
        b.Property(x => x.Email).HasMaxLength(160);
        b.Property(x => x.Phone).HasMaxLength(20);
        b.Property(x => x.Website).HasMaxLength(160);
        b.Property(x => x.AddressLine).HasMaxLength(300);
        b.Property(x => x.City).HasMaxLength(80);
        b.Property(x => x.State).HasMaxLength(80);
        b.Property(x => x.StateCode).HasMaxLength(4);
        b.Property(x => x.Pincode).HasMaxLength(10);
    }
}

/// <summary>Business settings mapping (singleton row).</summary>
public class BusinessSettingsConfiguration : IEntityTypeConfiguration<BusinessSettings>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BusinessSettings> b)
    {
        b.ToTable("BusinessSettings");
        b.HasIndex(s => s.BusinessId).IsUnique().HasFilter("[BusinessId] IS NOT NULL");
        b.Property(s => s.InvoicePrefix).HasMaxLength(20).IsRequired();
        b.Property(s => s.InvoiceFooter).HasMaxLength(500);
        b.Property(s => s.InvoiceTerms).HasMaxLength(2000);
        b.Property(s => s.DefaultPrinter).HasMaxLength(120);
        b.Property(s => s.BackupLocation).HasMaxLength(400);
    }
}

/// <summary>Financial year mapping.</summary>
public class FinancialYearConfiguration : IEntityTypeConfiguration<FinancialYear>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<FinancialYear> b)
    {
        b.ToTable("FinancialYears");
        b.Property(f => f.Name).HasMaxLength(40).IsRequired();
        b.HasIndex(f => new { f.StartDate, f.EndDate });
    }
}

/// <summary>Setting entries mapping.</summary>
public class SettingEntryConfiguration : IEntityTypeConfiguration<SettingEntry>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SettingEntry> b)
    {
        b.ToTable("SettingEntries");
        b.HasIndex(s => s.Key).IsUnique();
        b.Property(s => s.Key).HasMaxLength(120).IsRequired();
        b.Property(s => s.Value).HasMaxLength(4000);
        b.Property(s => s.Description).HasMaxLength(300);
    }
}

/// <summary>Audit log mapping.</summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.ToTable("AuditLogs");
        b.HasIndex(a => new { a.TimestampUtc });
        b.HasIndex(a => new { a.Module, a.Action });
        b.HasIndex(a => a.UserId);
        b.Property(a => a.Action).HasMaxLength(60).IsRequired();
        b.Property(a => a.Module).HasMaxLength(40);
        b.Property(a => a.Entity).HasMaxLength(60);
        b.Property(a => a.EntityId).HasMaxLength(50);
        b.Property(a => a.Username).HasMaxLength(60);
        b.Property(a => a.OldValues).HasMaxLength(4000);
        b.Property(a => a.NewValues).HasMaxLength(4000);
    }
}

/// <summary>Expense mapping.</summary>
public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Expense> b)
    {
        b.ToTable("Expenses");
        b.HasIndex(e => e.ExpenseNumber).IsUnique();
        b.Property(e => e.ExpenseNumber).HasMaxLength(40).IsRequired();
        b.Property(e => e.CategoryName).HasMaxLength(120).IsRequired();
        b.Property(e => e.PaidTo).HasMaxLength(160);
        b.Property(e => e.Amount).HasPrecision(18, 2);
        b.Property(e => e.TaxAmount).HasPrecision(18, 2);
        b.Property(e => e.Reference).HasMaxLength(60);
        b.Property(e => e.Description).HasMaxLength(1000);
        b.Property(e => e.AttachmentPath).HasMaxLength(400);
        b.HasIndex(e => new { e.DateUtc });
        b.HasOne(e => e.Category).WithMany().OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>Expense category mapping.</summary>
public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ExpenseCategory> b)
    {
        b.ToTable("ExpenseCategories");
        b.HasIndex(c => c.Name).IsUnique();
        b.Property(c => c.Name).HasMaxLength(120).IsRequired();
    }
}
