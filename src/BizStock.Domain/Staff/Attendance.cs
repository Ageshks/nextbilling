using BizStock.Domain.Common;

namespace BizStock.Domain.Staff;

/// <summary>Daily attendance record for an employee.</summary>
public class Attendance : BaseEntity
{
    /// <summary>Employee id.</summary>
    public Guid EmployeeId { get; set; }

    /// <summary>Attendance date.</summary>
    public DateOnly Date { get; set; }

    /// <summary>Check-in time.</summary>
    public TimeOnly? CheckIn { get; set; }

    /// <summary>Check-out time.</summary>
    public TimeOnly? CheckOut { get; set; }

    /// <summary>Present / absent / leave…</summary>
    public AttendanceStatus Status { get; set; }

    /// <summary>Leave type when on leave.</summary>
    public LeaveType? LeaveType { get; set; }

    /// <summary>Extra hours worked (overtime).</summary>
    public decimal OvertimeHours { get; set; }

    /// <summary>Note.</summary>
    public string? Note { get; set; }

    /// <summary>Employee navigation.</summary>
    public Employee Employee { get; set; } = default!;
}

/// <summary>Attendance day categories.</summary>
public enum AttendanceStatus
{
    /// <summary>Present full day.</summary>
    Present = 1,

    /// <summary>Absent.</summary>
    Absent = 2,

    /// <summary>Paid leave.</summary>
    PaidLeave = 3,

    /// <summary>Unpaid leave.</summary>
    UnpaidLeave = 4,

    /// <summary>Half day (half pay).</summary>
    HalfDay = 5,

    /// <summary>Public holiday (paid).</summary>
    Holiday = 6,

    /// <summary>Weekly off (paid).</summary>
    WeeklyOff = 7
}

/// <summary>Types of leave.</summary>
public enum LeaveType
{
    /// <summary>Casual leave.</summary>
    Casual = 1,

    /// <summary>Sick leave.</summary>
    Sick = 2,

    /// <summary>Earned/privileged leave.</summary>
    Earned = 3,

    /// <summary>Leave without pay.</summary>
    Unpaid = 4,

    /// <summary>Compensatory off.</summary>
    CompOff = 5,

    /// <summary>Maternity/paternity.</summary>
    Maternity = 6
}

/// <summary>Leave request aggregate.</summary>
public class LeaveRequest : BaseEntity
{
    /// <summary>Employee id.</summary>
    public Guid EmployeeId { get; set; }

    /// <summary>Leave type.</summary>
    public LeaveType LeaveType { get; set; }

    /// <summary>Inclusive start date.</summary>
    public DateOnly FromDate { get; set; }

    /// <summary>Inclusive end date.</summary>
    public DateOnly ToDate { get; set; }

    /// <summary>Total days requested.</summary>
    public decimal Days { get; set; }

    /// <summary>Reason.</summary>
    public string? Reason { get; set; }

    /// <summary>Pending / approved / rejected / cancelled.</summary>
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

    /// <summary>Approver user id.</summary>
    public Guid? ApprovedByUserId { get; set; }

    /// <summary>Approval decision timestamp (UTC).</summary>
    public DateTime? DecidedAtUtc { get; set; }

    /// <summary>Approver remark.</summary>
    public string? DecisionNote { get; set; }

    /// <summary>Employee navigation.</summary>
    public Employee Employee { get; set; } = default!;
}

/// <summary>Leave request states.</summary>
public enum LeaveStatus
{
    /// <summary>Awaiting decision.</summary>
    Pending = 0,

    /// <summary>Approved.</summary>
    Approved = 1,

    /// <summary>Rejected.</summary>
    Rejected = 2,

    /// <summary>Cancelled by employee.</summary>
    Cancelled = 3
}
