using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BizStock.Infrastructure.Persistence;

/// <summary>
/// Design-time factory so `dotnet ef` can create migrations without launching
/// the WPF application.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <inheritdoc />
    public AppDbContext CreateDbContext(string[] args)
    {
        DatabasePaths.EnsureDataDirectory();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(DatabasePaths.GetConnectionString())
            .Options;
        return new AppDbContext(options);
    }
}
