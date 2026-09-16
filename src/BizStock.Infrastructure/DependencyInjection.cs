using BizStock.Application.Billing;
using BizStock.Application.Common.Interfaces;
using BizStock.Application.Common.Licensing;
using BizStock.Infrastructure.Persistence;
using BizStock.Infrastructure.Security;
using BizStock.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BizStock.Infrastructure;

/// <summary>Dependency registration for the infrastructure layer.</summary>
public static class DependencyInjection
{
    /// <summary>Registers database, unit of work and application services.</summary>
    public static IServiceCollection AddBizStockInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dataDirectory = configuration["DataDirectory"] ?? DatabasePaths.GetDefaultDataDirectory();
        DatabasePaths.EnsureDataDirectory(dataDirectory);
        var connectionString = DatabasePaths.GetConnectionString(dataDirectory);

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBusinessContext, BusinessContext>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<INumberSequenceService, NumberSequenceService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<IAccountingService, AccountingService>();
        services.AddScoped<IBackupService, SqliteBackupService>();
        services.AddScoped<ILicenseService, LocalLicenseService>();
        services.AddScoped<BillingService>();

        // Read models / query services.
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IReportService, ReportService>();

        // Session state must outlive the per-use-case scopes that the desktop shell
        // creates for each screen, hence a singleton.
        services.AddSingleton<ISessionService, SessionService>();

        services.AddScoped<DatabaseSeeder>();

        return services;
    }

    /// <summary>
    /// Applies migrations and seeds reference data during controlled startup.
    /// Never drops or recreates the database, so existing business data is preserved.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.InitializeAsync(ct);
    }
}
