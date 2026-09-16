using BizStock.Domain.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BizStock.Infrastructure.Persistence.Configurations;

/// <summary>Account mapping.</summary>
public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Account> b)
    {
        b.ToTable("Accounts");
        b.HasIndex(a => a.Code).IsUnique();
        b.Property(a => a.Code).HasMaxLength(20).IsRequired();
        b.Property(a => a.Name).HasMaxLength(120).IsRequired();
    }
}

/// <summary>Journal entry mapping.</summary>
public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<JournalEntry> b)
    {
        b.ToTable("JournalEntries");
        b.HasIndex(j => j.VoucherNumber).IsUnique();
        b.HasIndex(j => new { j.DateUtc, j.VoucherType });
        b.HasIndex(j => new { j.DocumentId });
        b.Property(j => j.VoucherNumber).HasMaxLength(40).IsRequired();
        b.Property(j => j.Reference).HasMaxLength(40);
        b.Property(j => j.Narration).HasMaxLength(500);
        b.Ignore(j => j.TotalDebit);
        b.Ignore(j => j.TotalCredit);
        b.HasMany(j => j.Lines).WithOne(l => l.JournalEntry).HasForeignKey(l => l.JournalEntryId).OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Journal line mapping.</summary>
public class JournalLineConfiguration : IEntityTypeConfiguration<JournalLine>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<JournalLine> b)
    {
        b.ToTable("JournalLines");
        b.Property(l => l.Debit).HasPrecision(18, 2);
        b.Property(l => l.Credit).HasPrecision(18, 2);
        b.Property(l => l.Narration).HasMaxLength(500);
        b.HasIndex(l => l.AccountId);
        b.HasOne(l => l.Account).WithMany().OnDelete(DeleteBehavior.Restrict);
    }
}
