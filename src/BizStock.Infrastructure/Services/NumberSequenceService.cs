using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Common;
using BizStock.Domain.Settings;
using Microsoft.EntityFrameworkCore;

using BizStock.Infrastructure.Persistence;
namespace BizStock.Infrastructure.Services;

/// <summary>
/// SQLite-backed sequence allocator. A single row per (document type, period key)
/// is incremented under a transaction; uniqueness of the index makes concurrent
/// allocation safe.
/// </summary>
public class NumberSequenceService : INumberSequenceService
{
    private readonly AppDbContext _db;
    private readonly IBusinessContext _business;

    /// <summary>Creates the sequence service.</summary>
    public NumberSequenceService(AppDbContext db, IBusinessContext business)
    {
        _db = db;
        _business = business;
    }

    /// <inheritdoc />
    public async Task<string> GetNextNumberAsync(DocumentType type, CancellationToken ct = default)
    {
        var (prefix, periodKey) = await GetPrefixAndPeriodAsync(type, ct);
        var next = await AllocateNextAsync(type, periodKey, ct);
        return $"{prefix}-{periodKey}-{next:D6}";
    }

    /// <inheritdoc />
    public async Task<string> PeekNextNumberAsync(DocumentType type, CancellationToken ct = default)
    {
        var (prefix, periodKey) = await GetPrefixAndPeriodAsync(type, ct);
        var last = await ReadCounterAsync(type, periodKey, ct);
        return $"{prefix}-{periodKey}-{last + 1:D6}";
    }

    private async Task<(string Prefix, string PeriodKey)> GetPrefixAndPeriodAsync(DocumentType type, CancellationToken ct)
    {
        var settings = await _business.GetSettingsAsync(ct);
        var business = await _business.GetBusinessAsync(ct);

        var prefix = type switch
        {
            DocumentType.SaleInvoice => settings.InvoicePrefix,
            DocumentType.Purchase => "PUR",
            DocumentType.SalesReturn => "SR",
            DocumentType.PurchaseReturn => "PR",
            DocumentType.StockAdjustment => "ADJ",
            DocumentType.JournalEntry => "JV",
            DocumentType.CustomerReceipt => "RCPT",
            DocumentType.SupplierPayment => "PAY",
            DocumentType.Customer => "C",
            DocumentType.Supplier => "S",
            DocumentType.Product => "P",
            DocumentType.HeldSale => "HOLD",
            _ => "DOC"
        };

        // Period key: financial-year based for documents, plain for masters.
        var yearly = type is DocumentType.SaleInvoice or DocumentType.Purchase
            or DocumentType.SalesReturn or DocumentType.PurchaseReturn
            or DocumentType.StockAdjustment or DocumentType.JournalEntry;

        if (!yearly)
        {
            return (prefix, "0000");
        }

        var (startYear, endYear) = ComputeFinancialYear(DateTime.UtcNow, business?.FinancialYearStartMonth ?? 4);
        return (prefix, $"{startYear % 100:D2}{endYear % 100:D2}");
    }

    /// <summary>Indian financial year: April–March by default.</summary>
    internal static (int StartYear, int EndYear) ComputeFinancialYear(DateTime utcNow, int startMonth)
    {
        var year = utcNow.Year;
        return utcNow.Month >= startMonth ? (year, year + 1) : (year - 1, year);
    }

    /// <summary>
    /// Reads then increments the counter row. Safe under the desktop single-writer
    /// model because the unit of work serializes transactional writes.
    /// </summary>
    private async Task<long> AllocateNextAsync(DocumentType type, string periodKey, CancellationToken ct)
    {
        var key = SequenceKey(type, periodKey);
        var entry = await _db.SettingEntries.FirstOrDefaultAsync(s => s.Key == key, ct);

        if (entry is null)
        {
            entry = new SettingEntry { Key = key, Value = "1" };
            _db.SettingEntries.Add(entry);
            return 1;
        }

        var current = long.TryParse(entry.Value, out var n) ? n : 0;
        entry.Value = (current + 1).ToString();
        return current + 1;
    }

    private async Task<long> ReadCounterAsync(DocumentType type, string periodKey, CancellationToken ct)
    {
        var key = SequenceKey(type, periodKey);
        var entry = await _db.SettingEntries.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key, ct);
        return entry?.Value is string v && long.TryParse(v, out var n) ? n : 0;
    }

    private static string SequenceKey(DocumentType type, string periodKey) =>
        $"Seq.{type}.{periodKey}";
}
