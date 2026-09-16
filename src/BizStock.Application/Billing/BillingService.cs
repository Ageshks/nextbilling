using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Catalog;
using BizStock.Domain.Common;
using BizStock.Domain.Sales;
using BizStock.Domain.ValueObjects;

namespace BizStock.Application.Billing;

/// <summary>
/// Billing engine: computes GST splits (CGST/SGST vs IGST), validates stock,
/// persists the sale with stock movements and customer ledger in one transaction.
/// </summary>
public partial class BillingService
{
    private readonly IAppDbContext _db;
    private readonly IUnitOfWork _uow;
    private readonly IStockService _stock;
    private readonly INumberSequenceService _sequences;
    private readonly IAccountingService _accounting;
    private readonly IAuditService _audit;
    private readonly IBusinessContext _business;

    /// <summary>Creates the billing service.</summary>
    public BillingService(
        IAppDbContext db,
        IUnitOfWork uow,
        IStockService stock,
        INumberSequenceService sequences,
        IAccountingService accounting,
        IAuditService audit,
        IBusinessContext business)
    {
        _db = db;
        _uow = uow;
        _stock = stock;
        _sequences = sequences;
        _accounting = accounting;
        _audit = audit;
        _business = business;
    }

    /// <summary>Computes bill totals without persisting anything (live cart preview).</summary>
    public async Task<BillTotals> ComputeTotalsAsync(IReadOnlyList<BillingLine> lines, decimal invoiceDiscount, CancellationToken ct = default)
    {
        var settings = await _business.GetSettingsAsync(ct);
        var business = await _business.GetBusinessAsync(ct);
        var intraState = string.IsNullOrEmpty(business?.StateCode); // default home-state assumption

        decimal subTotal = 0, lineDiscounts = 0, cgst = 0, sgst = 0, igst = 0;

        foreach (var line in lines)
        {
            var gross = Money.Round(line.Quantity * line.UnitPrice);
            var discount = GetLineDiscount(line, gross);
            var taxable = Money.Round(gross - discount);

            subTotal += gross;
            lineDiscounts += discount;

            var (tax, c, s, i) = Sale.ComputeGST(taxable, line.GSTRate, intraState);
            cgst += c;
            sgst += s;
            igst += i;
        }

        return FinalizeTotals(settings.RoundOffEnabled,
            Money.Round(subTotal), Money.Round(lineDiscounts), Money.Round(invoiceDiscount),
            Money.Round(cgst), Money.Round(sgst), Money.Round(igst));
    }

    /// <summary>Effective discount for a line: flat amount wins over percent.</summary>
    private static decimal GetLineDiscount(BillingLine line, decimal gross) =>
        line.DiscountAmount > 0
            ? Money.Round(line.DiscountAmount)
            : Money.Round(gross * line.DiscountPercent / 100m);

    /// <summary>Aggregates sub totals into final totals with rounding.</summary>
    private static BillTotals FinalizeTotals(bool roundOffEnabled, decimal subTotal, decimal lineDiscounts,
        decimal invoiceDiscount, decimal cgst, decimal sgst, decimal igst)
    {
        var taxableTotal = Money.Round(subTotal - lineDiscounts - invoiceDiscount);
        var totalTax = Money.Round(cgst + sgst + igst);
        var grand = Money.Round(taxableTotal + totalTax);

        decimal roundOff = 0;
        if (roundOffEnabled)
        {
            var rounded = Math.Round(grand, MidpointRounding.AwayFromZero);
            roundOff = Money.Round(rounded - grand);
            grand = rounded;
        }

        return new BillTotals(subTotal, lineDiscounts, invoiceDiscount, taxableTotal, cgst, sgst, igst, totalTax, grand, roundOff);
    }
}
