using BizStock.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BizStock.Infrastructure.Persistence.Configurations;

/// <summary>Brand mapping.</summary>
public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Brand> b)
    {
        b.ToTable("Brands");
        b.HasIndex(x => x.Name).IsUnique();
        b.Property(x => x.Name).HasMaxLength(120).IsRequired();
    }
}

/// <summary>Unit mapping.</summary>
public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Unit> b)
    {
        b.ToTable("Units");
        b.HasIndex(x => x.Name).IsUnique();
        b.HasIndex(x => x.Symbol).IsUnique();
        b.Property(x => x.Name).HasMaxLength(60).IsRequired();
        b.Property(x => x.Symbol).HasMaxLength(20).IsRequired();
    }
}

/// <summary>TaxRate mapping.</summary>
public class TaxRateConfiguration : IEntityTypeConfiguration<TaxRate>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TaxRate> b)
    {
        b.ToTable("TaxRates");
        b.Property(x => x.Name).HasMaxLength(60).IsRequired();
        b.Property(x => x.RatePercent).HasPrecision(5, 2);
    }
}
