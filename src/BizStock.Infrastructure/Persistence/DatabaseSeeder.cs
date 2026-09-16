using BizStock.Domain.Business;
using BizStock.Domain.Catalog;
using BizStock.Domain.Identity;
using BizStock.Domain.Expenses;
using BizStock.Domain.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BizStock.Infrastructure.Persistence;

/// <summary>
/// Applies migrations and seeds reference data. Idempotent: existing rows are
/// never overwritten, so upgrading a live database preserves all business data.
/// </summary>
public partial class DatabaseSeeder
{
    private readonly AppDbContext _db;
    private readonly ILogger<DatabaseSeeder> _logger;

    /// <summary>Creates the seeder.</summary>
    public DatabaseSeeder(AppDbContext db, ILogger<DatabaseSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>Migrates the schema to the latest version and seeds reference data.</summary>
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        DatabasePaths.EnsureDataDirectory();
        await _db.Database.MigrateAsync(ct);
        await SeedPermissionsAndRolesAsync(ct);
        await SeedDefaultsAsync(ct);
        _logger.LogInformation("Database initialized at {Path}", DatabasePaths.GetDatabasePath());
    }

    private async Task SeedPermissionsAndRolesAsync(CancellationToken ct)
    {
        var existingKeys = await _db.Permissions.Select(p => p.Key).ToListAsync(ct);
        var newPermissions = new List<Permission>();
        foreach (var module in PermissionModules.All)
        {
            foreach (var action in Enum.GetValues<PermissionAction>())
            {
                var key = PermissionKeys.For(module, action);
                if (!existingKeys.Contains(key))
                {
                    newPermissions.Add(new Permission
                    {
                        Key = key,
                        Module = module,
                        Action = action,
                        DisplayName = $"{module} {action}"
                    });
                }
            }
        }

        if (newPermissions.Count > 0)
        {
            _db.Permissions.AddRange(newPermissions);
            await _db.SaveChangesAsync(ct);
        }
    }
}
