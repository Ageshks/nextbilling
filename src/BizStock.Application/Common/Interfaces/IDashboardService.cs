namespace BizStock.Application.Common.Interfaces;

/// <summary>One point on a dashboard trend chart.</summary>
public sealed record TrendPoint(DateOnly Date, decimal Sales, decimal Purchases, decimal Expenses, decimal Profit);

/// <summary>
/// Executive dashboard aggregates. Every figure is computed from posted
/// transactions (sales, purchases, expenses, stock ledger, journals) – never from
/// cached counters that can drift.
/// </summary>
public sealed record DashboardSummary(
    DateOnly FromDate,
    DateOnly ToDate,
    decimal SalesRevenue,
    int SalesInvoiceCount,
    decimal SalesReturnsAmount,
    decimal PurchaseValue,
    int PurchaseCount,
    decimal PurchaseReturnsAmount,
    decimal ExpenseAmount,
    decimal CostOfGoodsSold,
    decimal GrossProfit,
    decimal NetProfit,
    decimal CashBalance,
    decimal BankBalance,
    decimal Receivables,
    decimal Payables,
    decimal StockValue,
    int LowStockCount,
    int ActiveEmployees,
    int PresentToday,
    int AbsentToday,
    decimal PayrollNetThisMonth,
    bool PayrollPendingThisMonth);

/// <summary>Dashboard read model provider.</summary>
public interface IDashboardService
{
    /// <summary>Headline cards for the given business-date range.</summary>
    Task<DashboardSummary> GetSummaryAsync(DateOnly fromDate, DateOnly toDate, CancellationToken ct = default);

    /// <summary>Daily trend series (sales, purchases, expenses, profit) for the range.</summary>
    Task<IReadOnlyList<TrendPoint>> GetTrendAsync(DateOnly fromDate, DateOnly toDate, CancellationToken ct = default);
}