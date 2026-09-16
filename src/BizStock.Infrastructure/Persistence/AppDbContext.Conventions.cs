using BizStock.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BizStock.Infrastructure.Persistence;

/// <summary>Save-time conventions of <see cref="AppDbContext"/>.</summary>
public partial class AppDbContext
{
    /// <summary>Normalizes timestamps and business scoping before persisting.</summary>
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
                entry.Entity.UpdatedAtUtc = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
            }
        }

        foreach (var entry in ChangeTracker.Entries<Domain.Common.IBusinessScoped>()
                     .Where(e => e.State == EntityState.Added))
        {
            entry.Entity.BusinessId ??= Businesses.Local.FirstOrDefault()?.Id;
        }

        return base.SaveChangesAsync(ct);
    }
}
