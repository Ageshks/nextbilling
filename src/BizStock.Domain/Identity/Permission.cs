using BizStock.Domain.Common;

namespace BizStock.Domain.Identity;

/// <summary>Action kinds granted by permissions.</summary>
public enum PermissionAction
{
    /// <summary>View data.</summary>
    View = 1,

    /// <summary>Create records.</summary>
    Create = 2,

    /// <summary>Edit records.</summary>
    Edit = 3,

    /// <summary>Delete/deactivate records.</summary>
    Delete = 4,

    /// <summary>Approve sensitive workflows.</summary>
    Approve = 5,

    /// <summary>Print documents.</summary>
    Print = 6,

    /// <summary>Export data.</summary>
    Export = 7
}

/// <summary>Modules that expose permission-controlled functionality.</summary>
public static class PermissionModules
{
    /// <summary>Billing / POS.</summary>
    public const string Billing = "Billing";

    /// <summary>Sales.</summary>
    public const string Sales = "Sales";

    /// <summary>Products.</summary>
    public const string Products = "Products";

    /// <summary>Inventory.</summary>
    public const string Inventory = "Inventory";

    /// <summary>Purchases.</summary>
    public const string Purchases = "Purchases";

    /// <summary>Customers.</summary>
    public const string Customers = "Customers";

    /// <summary>Suppliers.</summary>
    public const string Suppliers = "Suppliers";

    /// <summary>Staff.</summary>
    public const string Staff = "Staff";

    /// <summary>Attendance.</summary>
    public const string Attendance = "Attendance";

    /// <summary>Leave.</summary>
    public const string Leave = "Leave";

    /// <summary>Payroll.</summary>
    public const string Payroll = "Payroll";

    /// <summary>Finance.</summary>
    public const string Finance = "Finance";

    /// <summary>Reports.</summary>
    public const string Reports = "Reports";

    /// <summary>Users.</summary>
    public const string Users = "Users";

    /// <summary>Settings.</summary>
    public const string Settings = "Settings";

    /// <summary>Backup.</summary>
    public const string Backup = "Backup";

    /// <summary>All module keys in display order.</summary>
    public static readonly string[] All =
    [
        Billing, Sales, Products, Inventory, Purchases, Customers, Suppliers,
        Staff, Attendance, Leave, Payroll, Finance, Reports, Users, Settings, Backup
    ];
}

/// <summary>A single grantable permission (module + action).</summary>
public class Permission : BaseEntity
{
    /// <summary>Unique permission key, e.g. "Products:Create".</summary>
    public string Key { get; set; } = default!;

    /// <summary>Module the permission belongs to.</summary>
    public string Module { get; set; } = default!;

    /// <summary>Action granted by the permission.</summary>
    public PermissionAction Action { get; set; }

    /// <summary>Friendly display name.</summary>
    public string DisplayName { get; set; } = default!;
}

/// <summary>Helpers to build and parse permission keys.</summary>
public static class PermissionKeys
{
    /// <summary>Builds a permission key such as "Products:Create".</summary>
    public static string For(string module, PermissionAction action) => $"{module}:{action}";

    /// <summary>Parses a permission key into its module and action parts.</summary>
    public static (string Module, PermissionAction Action) Parse(string key)
    {
        var parts = key.Split(':', 2);
        if (parts.Length != 2 || !Enum.TryParse(parts[1], out PermissionAction action))
        {
            throw new FormatException($"Invalid permission key '{key}'.");
        }

        return (parts[0], action);
    }
}
