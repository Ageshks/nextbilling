using BizStock.Domain.Catalog;
using BizStock.Domain.Expenses;
using BizStock.Domain.Finance;
using BizStock.Domain.Identity;
using BizStock.Domain.Business;
using Microsoft.EntityFrameworkCore;

namespace BizStock.Infrastructure.Persistence;

/// <summary>Reference-data seeding continued: roles, business, masters, chart of accounts.</summary>
public partial class DatabaseSeeder
{
    private async Task SeedRolesAsync(CancellationToken ct)
    {
        var allPermissions = await _db.Permissions.ToListAsync(ct);
        var existingRoles = await _db.Roles.Include(r => r.RolePermissions).ToListAsync(ct);

        void EnsureRole(string name, string description, IEnumerable<string> modules, bool fullAccess)
        {
            if (existingRoles.Any(r => r.Name == name))
            {
                return;
            }

            var role = new Role { Name = name, Description = description, IsSystemRole = true };
            role.RolePermissions.AddRange(allPermissions
                .Where(p => fullAccess || modules.Contains(p.Module))
                .Select(p => new RolePermission { PermissionId = p.Id }));
            _db.Roles.Add(role);
        }

        EnsureRole("Administrator", "Full unrestricted access", Array.Empty<string>(), true);
        EnsureRole("Manager", "Operational management access",
            new[] { "Billing", "Sales", "Products", "Inventory", "Purchases", "Customers", "Suppliers", "Finance", "Reports", "Staff", "Attendance", "Leave", "Payroll" }, false);
        EnsureRole("Cashier", "Counter billing access",
            new[] { "Billing", "Products", "Customers" }, false);
        EnsureRole("Inventory Manager", "Stock and product access",
            new[] { "Products", "Inventory", "Purchases", "Suppliers" }, false);
        EnsureRole("Accountant", "Finance and reporting access",
            new[] { "Finance", "Reports", "Purchases", "Sales", "Customers", "Suppliers", "Backup" }, false);
        EnsureRole("Supervisor", "Supervisory access",
            new[] { "Billing", "Sales", "Reports", "Inventory", "Staff", "Attendance" }, false);

        await _db.SaveChangesAsync(ct);
    }

    private async Task SeedDefaultsAsync(CancellationToken ct)
    {
        await SeedRolesAsync(ct);

        if (!await _db.Businesses.AnyAsync(ct))
        {
            _db.Businesses.Add(new Business { Name = "My Business", IsSetupCompleted = false });
        }

        if (!await _db.BusinessSettings.AnyAsync(ct))
        {
            _db.BusinessSettings.Add(new BusinessSettings());
        }

        await SeedCatalogMastersAsync(ct);
        await SeedChartOfAccountsAsync(ct);

        await _db.SaveChangesAsync(ct);
    }
}
