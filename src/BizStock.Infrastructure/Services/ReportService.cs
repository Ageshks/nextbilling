using BizStock.Application.Common;
using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Common;
using BizStock.Domain.Sales;
using BizStock.Domain.ValueObjects;
using BizStock.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BizStock.Infrastructure.Services;

/// <summary>
/// Reporting engine. Every figure is projected from posted transaction tables
/// (sales, sale lines, stock ledger, expenses) with server-side aggregation and
/// narrow column projections – report screens never load whole entities.
/// </summary>
public sealed class ReportService : IReportService
{
    private readonly AppDbContext _db;

    /// <summary>Creates the report service.</summary>
    public ReportService(AppDbContext db) => _db = db;

    /// <inheritdoc />
    public async Task<SalesSummaryDto> GetSalesSummaryAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
    {
        var q = CompletedSales(fromUtc, toUtc);

        var count = await q.CountAsync(ct);
        if (count == 0)
        {
            return new SalesSummaryDto(0, 0m, 0m, 0m, 0m);
        }

        var gross = await q.SumAsync(s => (decimal?)s.SubTotal, ct) ?? 0m;
        var discounts = await q.SumAsync(s => (decimal?)s.DiscountAmount, ct) ?? 0m;
        var tax = await q.SumAsync(s => (decimal?)s.TaxAmount, ct) ?? 0m;
        var net = await q.SumAsync(s => (decimal?)s.TaxableAmount, ct) ?? 0m;

        return new SalesSummaryDto(count, Money.Round(gross), Money.Round(discounts), Money.Round(tax), Money.Round(net));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DailySalesDto>> GetDailySalesAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
    {
        // Project only the columns the chart needs, then bucket by local business date.
        // Row count is bounded by the requested report window.
        var rows = await CompletedSales(fromUtc, toUtc)
            .Select(s => new { s.DateUtc, s.TaxableAmount, s.PaymentMethod })
            .ToListAsync(ct);

        return rows
            .GroupBy(r => BusinessTime.ToLocalDate(r.DateUtc))
            .OrderBy(g => g.Key)
            .Select(g => new DailySalesDto(
                g.Key,
                g.Count(),
                Money.Round(g.Sum(x => x.TaxableAmount)),
                Money.Round(g.Where(x => x.PaymentMethod == PaymentMethod.Cash).Sum(x => x.TaxableAmount)),
                Money.Round(g.Where(x => x.PaymentMethod == PaymentMethod.Upi).Sum(x => x.TaxableAmount)),
                Money.Round(g.Where(x => x.PaymentMethod == PaymentMethod.Card).Sum(x => x.TaxableAmount)),
                Money.Round(g.Where(x => x.PaymentMethod == PaymentMethod.Credit).Sum(x => x.TaxableAmount))))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TopProductDto>> GetTopProductsAsync(DateTime fromUtc, DateTime toUtc, int take, CancellationToken ct = default)
    {
        if (take <= 0)
        {
            take = 10;
        }

        // Aggregate in SQL: group sale lines by product, then trim to the top N.
        // Cost is stored on the line, so margin is exact rather than re-derived from
        // today's product master (which may have been re-priced since the sale).
        var rows = await (
                from s in CompletedSales(fromUtc, toUtc)
                join i in _db.SaleItems.AsNoTracking() on s.Id equals i.SaleId
                group i by new { i.ProductId, i.ProductName } into g
                select new
                {
                    g.Key.ProductId,
                    g.Key.ProductName,
                    Quantity = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.LineTotal),
                    Cost = g.Sum(x => x.CostPrice * x.Quantity)
                })
            .OrderByDescending(x => x.Revenue)
            .Take(take)
            .ToListAsync(ct);

        var codes = await _db.Products.AsNoTracking()
            .Where(p => rows.Select(r => r.ProductId).Contains(p.Id))
            .Select(p => new { p.Id, p.ProductCode })
            .ToDictionaryAsync(p => p.Id, p => p.ProductCode, ct);

        return rows
            .Select(r => new TopProductDto(
                codes.TryGetValue(r.ProductId, out var code) ? code : "-",
                r.ProductName,
                r.Quantity,
                Money.Round(r.Revenue),
                Money.Round(r.Revenue - r.Cost)))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GstSlabSummaryDto>> GetGstSummaryAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
    {
        // The intra/inter-state decision is snapshotted on the invoice, so the split is
        // computed from what was actually charged rather than re-derived from master data.
        var lines = await (
                from s in CompletedSales(fromUtc, toUtc)
                join i in _db.SaleItems.AsNoTracking() on s.Id equals i.SaleId
                select new
                {
                    i.GSTRate,
                    i.TaxableAmount,
                    i.TaxAmount,
                    IsIntra = string.IsNullOrEmpty(s.CustomerStateCode)
                        || string.IsNullOrEmpty(s.BusinessStateCode)
                        || s.CustomerStateCode == s.BusinessStateCode
                })
            .ToListAsync(ct);

        return lines
            .GroupBy(l => l.GSTRate)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var taxable = Money.Round(g.Sum(x => x.TaxableAmount));
                var cgst = Money.Round(g.Where(x => x.IsIntra).Sum(x => x.TaxAmount) / 2m);
                var sgst = cgst;
                var igst = Money.Round(g.Where(x => !x.IsIntra).Sum(x => x.TaxAmount));
                return new GstSlabSummaryDto(g.Key, taxable, cgst, sgst, igst, Money.Round(cgst + sgst + igst));
            })
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StockValuationDto>> GetStockValuationAsync(CancellationToken ct = default)
    {
        var rows = await _db.StockSummaries.AsNoTracking()
            .Where(s => s.Quantity != 0)
            .Select(s => new
            {
                s.Product.ProductCode,
                s.Product.Name,
                s.Quantity,
                s.AverageCost,
                s.Product.SellingPrice
            })
            .ToListAsync(ct);

        return rows
            .Select(r => new StockValuationDto(
                r.ProductCode,
                r.Name,
                r.Quantity,
                Money.Round(r.AverageCost),
                Money.Round(r.Quantity * r.AverageCost),
                Money.Round(r.Quantity * r.SellingPrice)))
            .OrderByDescending(r => r.StockValue)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<ProfitAndLossDto> GetProfitAndLossAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
    {
        // Revenue is the net taxable value of completed invoices (already inclusive of
        // invoice-level discounts), which is what the GST returns report.
        var revenue = await CompletedSales(fromUtc, toUtc)
            .SumAsync(s => (decimal?)s.TaxableAmount, ct) ?? 0m;

        // COGS uses the unit cost frozen on each sale line at billing time.
        var cogs = await (
                from s in CompletedSales(fromUtc, toUtc)
                join i in _db.SaleItems.AsNoTracking() on s.Id equals i.SaleId
                select i.CostPrice * i.Quantity)
            .SumAsync(x => (decimal?)x, ct) ?? 0m;

        var expenses = await _db.Expenses.AsNoTracking()
            .Where(e => e.DateUtc >= fromUtc && e.DateUtc < toUtc)
            .SumAsync(e => (decimal?)e.Amount, ct) ?? 0m;

        var gross = revenue - cogs;
        return new ProfitAndLossDto(
            Money.Round(revenue),
            Money.Round(cogs),
            Money.Round(gross),
            Money.Round(expenses),
            Money.Round(gross - expenses));
    }

    /// <summary>Base query for completed sales in a half-open UTC window.</summary>
    private IQueryable<Sale> CompletedSales(DateTime fromUtc, DateTime toUtc) =>
        _db.Sales.AsNoTracking().Where(s =>
            s.SaleStatus == SaleStatus.Completed && s.DateUtc >= fromUtc && s.DateUtc < toUtc);
}