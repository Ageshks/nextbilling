using BizStock.Domain.Common;

namespace BizStock.Domain.Staff;

/// <summary>Employee master record.</summary>
public class Employee : BaseEntity, IActivatable
{
    /// <summary>Unique employee code, e.g. E0001.</summary>
    public string EmployeeCode { get; set; } = default!;

    /// <summary>Full name.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Linked login user id, if any.</summary>
    public Guid? UserId { get; set; }

    /// <summary>Designation.</summary>
    public string? Designation { get; set; }

    /// <summary>Department.</summary>
    public string? Department { get; set; }

    /// <summary>Mobile number.</summary>
    public string? Phone { get; set; }

    /// <summary>Email.</summary>
    public string? Email { get; set; }

    /// <summary>Street address.</summary>
    public string? Address { get; set; }

    /// <summary>Joining date (UTC).</summary>
    public DateTime JoinDateUtc { get; set; }

    /// <summary>Exit date (UTC) when no longer employed.</summary>
    public DateTime? ExitDateUtc { get; set; }

    /// <summary>Monthly gross salary.</summary>
    public decimal MonthlySalary { get; set; }

    /// <summary>Per-day salary derived from MonthlySalary / settings.WorkingDaysPerMonth.</summary>
    public decimal DailyRate { get; set; }

    /// <summary>Per-hour salary derived from DailyRate / 8.</summary>
    public decimal HourlyRate { get; set; }

    /// <summary>Standard daily working hours.</summary>
    public decimal WorkingHoursPerDay { get; set; } = 8;

    /// <summary>Whether the employee is currently employed.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Bank account number.</summary>
    public string? BankAccount { get; set; }

    /// <summary>Bank name.</summary>
    public string? BankName { get; set; }

    /// <summary>Optional notes.</summary>
    public string? Notes { get; set; }
}
