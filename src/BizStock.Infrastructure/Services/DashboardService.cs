using BizStock.Application.Common;
using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Common;
using BizStock.Domain.Finance;
using BizStock.Domain.Purchases;
using BizStock.Domain.Sales;
using BizStock.Domain.Staff;
using BizStock.Domain.ValueObjects;
using BizStock.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BizStock.Infrastructure.Services;

/// <summary>
/// Executive dashboard aggregates. Every card is a live query over posted
/// transactions – nothing is cached, so the numbers can never drift from the books.
/// Queries aggregate and project in SQL and never materialise whole tables.
/// </summary>
public sealed class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    /// <summary>Creates the dashboard service.</summary>
    public DashboardService(AppDbContext db) => _db = db;

    /// <inheritdoc />
    public async Task<DashboardSummary> GetSummaryAsync(DateOnly fromDate, DateOnly toDate, CancellationToken ct = default)
    {
        var (fromUtc, toUtc) = BusinessTime.RangeUtc(fromDate, toDate);

        var sales = await CompletedSales(fromUtc, toUtc)
            .GroupBy(_ => 1)
            .Select(g => new { Count = g.Count(), Net = g.Sum(s => s.TaxableAmount) })
            .FirstOrDefaultAsync(ct);

        var purchases = await _db.Purchases.AsNoTracking()
            .Where(p => p.PurchaseStatus == PurchaseStatus.Completed && p.DateUtc >= fromUtc && p.DateUtc < toUtc)
            .GroupBy(_ => 1)
            .Select(g => new { Count = g.Count(), Value = g.Sum(p => p.GrandTotal) })
            .FirstOrDefaultAsync(ct);

        var cogs = await (
                from s in CompletedSales(fromUtc, toUtc)
                join i in _db.SaleItems.AsNoTracking() on s.Id equals i.SaleId
                select i.CostPrice * i.Quantity)
            .SumAsync(x => (decimal?)x, ct) ?? 0m;

        var expenses = await _db.Expenses.AsNoTracking()
            .Where(e => e.DateUtc >= fromUtc && e.DateUtc < toUtc)
            .SumAsync(e => (decimal?)e.Amount, ct) ?? 0m;

        var salesReturns = await _db.SalesReturns.AsNoTracking()
            .Where(r => r.DateUtc >= fromUtc && r.DateUtc < toUtc)
            .SumAsync(r => (decimal?)r.TotalAmount, ct) ?? 0m;

        var purchaseReturns = await _db.PurchaseReturns.AsNoTracking()
            .Where(r => r.DateUtc >= fromUtc && r.DateUtc < toUtc)
            .SumAsync(r => (decimal?)r.TotalAmount, ct) ?? 0m;

        var stock = await _db.StockSummaries.AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Value = g.Sum(s => s.Quantity * s.AverageCost),
                Low = g.Count(s => s.Quantity <= s.Product.MinimumStock)
            })
            .FirstOrDefaultAsync(ct);

        var receivables = await _db.Customers.AsNoTracking()
            .SumAsync(c => (decimal?)c.OutstandingBalance, ct) ?? 0m;

        var payables = await _db.Suppliers.AsNoTracking()
            .SumAsync(s => (decimal?)s.OutstandingBalance, ct) ?? 0m;

        var cash = await AccountBalanceAsync(Account.SystemCodes.Cash, ct);
        var bank = await AccountBalanceAsync(Account.SystemCodes.Bank, ct);
var today = BusinessTime.Today;
        var attendance = await _db.Attendances.AsNoTracking()
            .Where(a => a.Date == today)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Present = g.Count(a => a.Status == AttendanceStatus.Present
                    || a.Status == AttendanceStatus.HalfDay
                    || a.Status == AttendanceStatus.PaidLeave
                    || a.Status == AttendanceStatus.Holiday
                    || a.Status == AttendanceStatus.WeeklyOff),
                Absent = g.Count(a => a.Status == AttendanceStatus.Absent)
            })
            .FirstOrDefaultAsync(ct);

        var activeEmployees = await _db.Employees.AsNoTracking().CountAsync(e => e.IsActive, ct);

        var month = today.Month;
        var year = today.Year;
        var payrollRun = await _db.PayrollRuns.AsNoTracking()
            .Where(r => r.Year == year && r.Month == month)
            .Select(r => new { r.TotalNet, r.IsPaid })
            .FirstOrDefaultAsync(ct);

        var netSales = sales?.Net ?? 0m;
        var grossProfit = netSales - cogs;

        return new DashboardSummary(
            fromDate,
            toDate,
            Money.Round(netSales),
            sales?.Count ?? 0,
            Money.Round(salesReturns),
            Money.Round(purchases?.Value ?? 0m),
            purchases?.Count ?? 0,
            Money.Round(purchaseReturns),
            Money.Round(expenses),
            Money.Round(cogs),
            Money.Round(grossProfit),
            Money.Round(grossProfit - expenses),
            Money.Round(cash),
            Money.Round(bank),
            Money.Round(receivables),
            Money.Round(payables),
            Money.Round(stock?.Value ?? 0m),
            stock?.Low ?? 0,
            activeEmployees,
            attendance?.Present ?? 0,
            attendance?.Absent ?? 0,
            Money.Round(payrollRun?.TotalNet ?? 0m),
            payrollRun is null || !payrollRun.IsPaid);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TrendPoint>> GetTrendAsync(DateOnly fromDate, DateOnly toDate, CancellationToken ct = default)
    {
        var (fromUtc, toUtc) = BusinessTime.RangeUtc(fromDate, toDate);

        var sales = await CompletedSales(fromUtc, toUtc)
            .Select(s => new { s.DateUtc, s.TaxableAmount })
            .ToListAsync(ct);

        var purchases = await _db.Purchases.AsNoTracking()
            .Where(p => p.PurchaseStatus == PurchaseStatus.Completed && p.DateUtc >= fromUtc && p.DateUtc < toUtc)
            .Select(p => new { p.DateUtc, p.GrandTotal })
            .ToListAsync(ct);

        var expenses = await _db.Expenses.AsNoTracking()
            .Where(e => e.DateUtc >= fromUtc && e.DateUtc < toUtc)
            .Select(e => new { e.DateUtc, e.Amount })
            .ToListAsync(ct);

        var cogs = await (
                from s in CompletedSales(fromUtc, toUtc)
                join i in _db.SaleItems.AsNoTracking() on s.Id equals i.SaleId
                select new { s.DateUtc, Cost = i.CostPrice * i.Quantity })
            .ToListAsync(ct);

        // One row per day in the requested window keeps the chart continuous even on
        // days with no activity.
        var salesByDay = sales.GroupBy(x => BusinessTime.ToLocalDate(x.DateUtc))
            .ToDictionary(g => g.Key, g => Money.Round(g.Sum(x => x.TaxableAmount)));
        var purchaseByDay = purchases.GroupBy(x => BusinessTime.ToLocalDate(x.DateUtc))
            .ToDictionary(g => g.Key, g => Money.Round(g.Sum(x => x.GrandTotal)));
        var expenseByDay = expenses.GroupBy(x => BusinessTime.ToLocalDate(x.DateUtc))
            .ToDictionary(g => g.Key, g => Money.Round(g.Sum(x => x.Amount)));
        var cogsByDay = cogs.GroupBy(x => BusinessTime.ToLocalDate(x.DateUtc))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Cost));

        var start = fromDate <= toDate ? fromDate : toDate;
        var end = fromDate <= toDate ? toDate : fromDate;

        var points = new List<TrendPoint>();
        const int maxDays = 400; // guard: never build an unbounded in-memory series
        for (var d = start; d <= end && points.Count < maxDays; d = d.AddDays(1))
        {
            var s = salesByDay.GetValueOrDefault(d);
            var p = purchaseByDay.GetValueOrDefault(d);
            var e = expenseByDay.GetValueOrDefault(d);
            var c = cogsByDay.GetValueOrDefault(d);
            points.Add(new TrendPoint(d, s, p, e, Money.Round(s - c - e)));
        }

        return points;
    }

    /// <summary>Signed ledger balance for a system account (debit − credit).</summary>
    private async Task<decimal> AccountBalanceAsync(string accountCode, CancellationToken ct)
    {
        var accountId = await _db.Accounts.AsNoTracking()
            .Where(a => a.Code == accountCode)
            .Select(a => (Guid?)a.Id)
            .FirstOrDefaultAsync(ct);

        if (accountId is null)
        {
            return 0m;
        }

        var debit = await _db.JournalLines.AsNoTracking()
            .Where(l => l.AccountId == accountId.Value)
            .SumAsync(l => (decimal?)l.Debit, ct) ?? 0m;

        var credit = await _db.JournalLines.AsNoTracking()
            .Where(l => l.AccountId == accountId.Value)
            .SumAsync(l => (decimal?)l.Credit, ct) ?? 0m;

        return debit - credit;
    }

    /// <summary>Base query for completed sales in a half-open UTC window.</summary>
    private IQueryable<Sale> CompletedSales(DateTime fromUtc, DateTime toUtc) =>
        _db.Sales.AsNoTracking().Where(s =>
            s.SaleStatus == SaleStatus.Completed && s.DateUtc >= fromUtc && s.DateUtc < toUtc);
}