using BizStock.Domain.Common;

namespace BizStock.Domain.Staff;

/// <summary>
/// Department master (Sales, Accounts, Inventory, HR…). Drives staff grouping and
/// the department payroll report. Employees keep a name snapshot so historical
/// payslips stay valid even if the department is later renamed or deactivated.
/// </summary>
public class Department : BaseEntity, IActivatable
{
    /// <summary>Unique department name.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Optional description.</summary>
    public string? Description { get; set; }

    /// <summary>Whether the department can still be assigned.</summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>Designation master (Manager, Accountant, Cashier…).</summary>
public class Designation : BaseEntity, IActivatable
{
    /// <summary>Unique designation name.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Optional description.</summary>
    public string? Description { get; set; }

    /// <summary>Whether the designation can still be assigned.</summary>
    public bool IsActive { get; set; } = true;
}