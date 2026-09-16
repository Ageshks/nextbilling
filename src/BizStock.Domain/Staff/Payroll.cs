using BizStock.Domain.Common;

namespace BizStock.Domain.Staff;

/// <summary>Monthly payroll run header.</summary>
public class PayrollRun : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique run number, e.g. PAY-202604-0001.</summary>
    public string RunNumber { get; set; } = default!;

    /// <summary>Payroll year.</summary>
    public int Year { get; set; }

    /// <summary>Payroll month (1-12).</summary>
    public int Month { get; set; }

    /// <summary>Run date (UTC).</summary>
    public DateTime RunDateUtc { get; set; }

    /// <summary>Gross salary total.</summary>
    public decimal TotalGross { get; set; }

    /// <summary>Deduction total.</summary>
    public decimal TotalDeductions { get; set; }

    /// <summary>Net payable total.</summary>
    public decimal TotalNet { get; set; }

    /// <summary>Whether the payroll has been paid out.</summary>
    public bool IsPaid { get; set; }

    /// <summary>Payout timestamp (UTC).</summary>
    public DateTime? PaidAtUtc { get; set; }

    /// <summary>Note.</summary>
    public string? Note { get; set; }

    /// <summary>Payslips.</summary>
    public List<Payslip> Payslips { get; set; } = [];
}

/// <summary>Individual employee payslip.</summary>
public class Payslip : BaseEntity
{
    /// <summary>Parent payroll run id.</summary>
    public Guid PayrollRunId { get; set; }

    /// <summary>Employee id.</summary>
    public Guid EmployeeId { get; set; }

    /// <summary>Employee name snapshot.</summary>
    public string EmployeeName { get; set; } = default!;

    /// <summary>Days present (incl. paid leave/holiday).</summary>
    public decimal DaysPresent { get; set; }

    /// <summary>Days absent.</summary>
    public decimal DaysAbsent { get; set; }

    /// <summary>Base earned salary for the period.</summary>
    public decimal BasicSalary { get; set; }

    /// <summary>Overtime earnings.</summary>
    public decimal OvertimeAmount { get; set; }

    /// <summary>Bonus.</summary>
    public decimal Bonus { get; set; }

    /// <summary>Deductions (unpaid leave, other).</summary>
    public decimal Deductions { get; set; }

    /// <summary>Advance recovery deducted this period.</summary>
    public decimal AdvanceRecovery { get; set; }

    /// <summary>Net payable.</summary>
    public decimal NetPay { get; set; }

    /// <summary>Note.</summary>
    public string? Note { get; set; }

    /// <summary>Employee navigation.</summary>
    public Employee Employee { get; set; } = default!;
}

/// <summary>Employee advance (loan) record with installment recovery.</summary>
public class SalaryAdvance : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Employee id.</summary>
    public Guid EmployeeId { get; set; }

    /// <summary>Advance date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Advance amount.</summary>
    public decimal Amount { get; set; }

    /// <summary>Remaining balance.</summary>
    public decimal Balance { get; set; }

    /// <summary>Installment recovery per payroll period.</summary>
    public decimal RecoveryPerPeriod { get; set; }

    /// <summary>Reason/purpose.</summary>
    public string? Reason { get; set; }

    /// <summary>Whether fully recovered.</summary>
    public bool IsSettled { get; set; }

    /// <summary>Employee navigation.</summary>
    public Employee Employee { get; set; } = default!;
}
