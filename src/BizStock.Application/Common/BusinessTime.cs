namespace BizStock.Application.Common;

/// <summary>
/// UTC &lt;-&gt; local business time conversions. Timestamps are persisted in UTC while
/// users think and report in local business time, so every date-range query routes
/// through this helper instead of calling <c>DateTime.Now</c> ad hoc.
/// </summary>
public static class BusinessTime
{
    /// <summary>Local today.</summary>
    public static DateOnly Today => DateOnly.FromDateTime(DateTime.Now);

    /// <summary>Converts a local date to the UTC instant at which that day starts.</summary>
    public static DateTime ToUtcStart(DateOnly date) =>
        DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Local).ToUniversalTime();

    /// <summary>Converts a local date to the UTC instant at which that day ends (exclusive upper bound).</summary>
    public static DateTime ToUtcEndExclusive(DateOnly date) =>
        ToUtcStart(date.AddDays(1));

    /// <summary>Half-open UTC range [from, to) covering the inclusive local dates from..to.</summary>
    public static (DateTime FromUtc, DateTime ToUtc) RangeUtc(DateOnly from, DateOnly to)
    {
        if (to < from)
        {
            (from, to) = (to, from);
        }

        return (ToUtcStart(from), ToUtcEndExclusive(to));
    }

    /// <summary>Converts a UTC timestamp to the local business date.</summary>
    public static DateOnly ToLocalDate(DateTime utc) =>
        DateOnly.FromDateTime(DateTime.SpecifyKind(utc, DateTimeKind.Utc).ToLocalTime());

    /// <summary>Converts a UTC timestamp to local business time.</summary>
    public static DateTime ToLocal(DateTime utc) =>
        DateTime.SpecifyKind(utc, DateTimeKind.Utc).ToLocalTime();

    /// <summary>Current UTC timestamp.</summary>
    public static DateTime UtcNow => DateTime.UtcNow;
}