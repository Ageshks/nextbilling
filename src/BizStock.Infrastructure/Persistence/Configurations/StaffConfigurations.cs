using BizStock.Domain.Staff;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BizStock.Infrastructure.Persistence.Configurations;

/// <summary>Employee mapping.</summary>
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.ToTable("Employees");
        b.HasIndex(e => e.EmployeeCode).IsUnique();
        b.Property(e => e.EmployeeCode).HasMaxLength(30).IsRequired();
        b.Property(e => e.Name).HasMaxLength(160).IsRequired();
        b.Property(e => e.MonthlySalary).HasPrecision(18, 2);
        b.Property(e => e.DailyRate).HasPrecision(18, 2);
        b.Property(e => e.HourlyRate).HasPrecision(18, 2);
        b.Property(e => e.WorkingHoursPerDay).HasPrecision(5, 2);
        b.Property(e => e.Notes).HasMaxLength(1000);
    }
}

/// <summary>Attendance mapping (unique per employee per day).</summary>
public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Attendance> b)
    {
        b.ToTable("Attendances");
        b.HasIndex(a => new { a.EmployeeId, a.Date }).IsUnique();
        b.Property(a => a.OvertimeHours).HasPrecision(6, 2);
        b.Property(a => a.Note).HasMaxLength(500);
        b.HasOne(a => a.Employee).WithMany().OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Leave request mapping.</summary>
public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<LeaveRequest> b)
    {
        b.ToTable("LeaveRequests");
        b.Property(l => l.Days).HasPrecision(6, 2);
        b.Property(l => l.Reason).HasMaxLength(1000);
        b.Property(l => l.DecisionNote).HasMaxLength(500);
        b.HasIndex(l => new { l.EmployeeId, l.FromDate });
        b.HasOne(l => l.Employee).WithMany().OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Payroll run mapping.</summary>
public class PayrollRunConfiguration : IEntityTypeConfiguration<PayrollRun>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PayrollRun> b)
    {
        b.ToTable("PayrollRuns");
        b.HasIndex(p => p.RunNumber).IsUnique();
        b.HasIndex(p => new { p.Year, p.Month }).IsUnique();
        b.Property(p => p.RunNumber).HasMaxLength(40).IsRequired();
        b.Property(p => p.TotalGross).HasPrecision(18, 2);
        b.Property(p => p.TotalDeductions).HasPrecision(18, 2);
        b.Property(p => p.TotalNet).HasPrecision(18, 2);
        b.Property(p => p.Note).HasMaxLength(1000);
        b.HasMany(p => p.Payslips).WithOne().HasForeignKey(s => s.PayrollRunId).OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Payslip mapping.</summary>
public class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Payslip> b)
    {
        b.ToTable("Payslips");
        b.Property(s => s.EmployeeName).HasMaxLength(160).IsRequired();
        b.Property(s => s.DaysPresent).HasPrecision(6, 2);
        b.Property(s => s.DaysAbsent).HasPrecision(6, 2);
        b.Property(s => s.BasicSalary).HasPrecision(18, 2);
        b.Property(s => s.OvertimeAmount).HasPrecision(18, 2);
        b.Property(s => s.Bonus).HasPrecision(18, 2);
        b.Property(s => s.Deductions).HasPrecision(18, 2);
        b.Property(s => s.AdvanceRecovery).HasPrecision(18, 2);
        b.Property(s => s.NetPay).HasPrecision(18, 2);
        b.Property(s => s.Note).HasMaxLength(500);
    }
}

/// <summary>Salary advance mapping.</summary>
public class SalaryAdvanceConfiguration : IEntityTypeConfiguration<SalaryAdvance>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SalaryAdvance> b)
    {
        b.ToTable("SalaryAdvances");
        b.Property(a => a.Amount).HasPrecision(18, 2);
        b.Property(a => a.Balance).HasPrecision(18, 2);
        b.Property(a => a.RecoveryPerPeriod).HasPrecision(18, 2);
        b.Property(a => a.Reason).HasMaxLength(500);
        b.HasOne(a => a.Employee).WithMany().OnDelete(DeleteBehavior.Cascade);
    }
}
