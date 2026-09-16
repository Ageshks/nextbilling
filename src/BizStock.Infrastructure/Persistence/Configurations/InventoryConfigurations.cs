using BizStock.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BizStock.Infrastructure.Persistence.Configurations;

/// <summary>Stock balance mapping.</summary>
public class StockSummaryConfiguration : IEntityTypeConfiguration<StockSummary>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<StockSummary> b)
    {
        b.ToTable("StockSummaries");
        b.HasIndex(s => s.ProductId).IsUnique();
        b.Property(s => s.Quantity).HasPrecision(18, 3);
        b.Property(s => s.ReservedQuantity).HasPrecision(18, 3);
        b.Property(s => s.AverageCost).HasPrecision(18, 2);
    }
}

/// <summary>Stock movement (ledger) mapping.</summary>
public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<StockMovement> b)
    {
        b.ToTable("StockMovements");
        b.HasIndex(m => new { m.ProductId, m.DateUtc });
        b.HasIndex(m => m.Reference);
        b.Property(m => m.Reference).HasMaxLength(40);
        b.Property(m => m.BatchNumber).HasMaxLength(40);
        b.Property(m => m.Note).HasMaxLength(500);
        b.Property(m => m.Quantity).HasPrecision(18, 3);
        b.Property(m => m.BalanceAfter).HasPrecision(18, 3);
        b.Property(m => m.UnitCost).HasPrecision(18, 2);
        b.HasOne(m => m.Product).WithMany().OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>Stock adjustment mapping.</summary>
public class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<StockAdjustment> b)
    {
        b.ToTable("StockAdjustments");
        b.HasIndex(a => a.AdjustmentNumber).IsUnique();
        b.Property(a => a.AdjustmentNumber).HasMaxLength(40).IsRequired();
        b.Property(a => a.Note).HasMaxLength(1000);
        b.HasMany(a => a.Items).WithOne().HasForeignKey(i => i.StockAdjustmentId).OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Stock adjustment line mapping.</summary>
public class StockAdjustmentItemConfiguration : IEntityTypeConfiguration<StockAdjustmentItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<StockAdjustmentItem> b)
    {
        b.ToTable("StockAdjustmentItems");
        b.Property(i => i.Quantity).HasPrecision(18, 3);
        b.Property(i => i.UnitCost).HasPrecision(18, 2);
        b.Property(i => i.Note).HasMaxLength(500);
    }
}
