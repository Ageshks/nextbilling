using BizStock.Domain.Common;

namespace BizStock.Domain.Reports;

/// <summary>Snapshot of a dashboard metric at computation time (optional cache of live aggregates).</summary>
public class DashboardSnapshot : BaseEntity
{
    /// <summary>Metric key, e.g. "Sales.Today".</summary>
    public string MetricKey { get; set; } = default!;

    /// <summary>Numeric metric value.</summary>
    public decimal Value { get; set; }

    /// <summary>Snapshot date.</summary>
    public DateOnly Date { get; set; }
}
