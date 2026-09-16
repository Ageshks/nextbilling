using BizStock.Domain.Catalog;
using BizStock.Domain.Common;
using BizStock.Domain.Expenses;
using BizStock.Domain.Finance;
using Microsoft.EntityFrameworkCore;

namespace BizStock.Infrastructure.Persistence;

/// <summary>Catalog masters and chart of accounts seeding.</summary>
public partial class DatabaseSeeder
{
    private async Task SeedCatalogMastersAsync(CancellationToken ct)
    {
        var unitSymbols = await _db.Units.Select(u => u.Symbol).ToListAsync(ct);
        var unitSpecs = new (string Name, string Symbol, bool Decimals)[]
        {
            ("Pieces", "PCS", false), ("Box", "BOX", false), ("Kilogram", "KG", true),
            ("Gram", "G", true), ("Litre", "L", true), ("Millilitre", "ML", true),
            ("Metre", "M", true), ("Pack", "PACK", false), ("Dozen", "DOZEN", false)
        };
        foreach (var (name, symbol, decimals) in unitSpecs)
        {
            if (!unitSymbols.Contains(symbol))
            {
                _db.Units.Add(new Unit { Name = name, Symbol = symbol, DecimalAllowed = decimals, IsSystem = true });
            }
        }

        var rates = await _db.TaxRates.Select(t => t.RatePercent).ToListAsync(ct);
        foreach (var rate in new decimal[] { 0, 5, 12, 18, 28 })
        {
            if (!rates.Contains(rate))
            {
                _db.TaxRates.Add(new TaxRate { Name = $"GST {rate:0.##}%", RatePercent = rate });
            }
        }

        var expenseCats = await _db.ExpenseCategories.Select(c => c.Name).ToListAsync(ct);
        foreach (var name in new[] { "Rent", "Electricity", "Transport", "Salaries", "Marketing", "Maintenance", "Other" })
        {
            if (!expenseCats.Contains(name))
            {
                _db.ExpenseCategories.Add(new ExpenseCategory { Name = name });
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    private async Task SeedChartOfAccountsAsync(CancellationToken ct)
    {
        var codes = await _db.Accounts.Select(a => a.Code).ToListAsync(ct);
        var accounts = new (string Code, string Name, AccountType Type)[]
        {
            (Account.SystemCodes.Cash, "Cash in Hand", AccountType.Asset),
            (Account.SystemCodes.Bank, "Bank Account", AccountType.Asset),
            (Account.SystemCodes.AccountsReceivable, "Accounts Receivable", AccountType.Asset),
            (Account.SystemCodes.Inventory, "Inventory", AccountType.Asset),
            (Account.SystemCodes.InputGST, "Input GST Credit", AccountType.Asset),
            (Account.SystemCodes.AccountsPayable, "Accounts Payable", AccountType.Liability),
            (Account.SystemCodes.OutputGST, "Output GST Payable", AccountType.Liability),
            (Account.SystemCodes.OwnerCapital, "Owner's Capital", AccountType.Equity),
            (Account.SystemCodes.OwnerDrawings, "Owner's Drawings", AccountType.Asset),
            (Account.SystemCodes.RetainedEarnings, "Retained Earnings", AccountType.Equity),
            (Account.SystemCodes.Sales, "Sales Revenue", AccountType.Income),
            (Account.SystemCodes.SalesReturns, "Sales Returns", AccountType.Expense),
            (Account.SystemCodes.OtherIncome, "Other Income", AccountType.Income),
            (Account.SystemCodes.CostOfGoodsSold, "Cost of Goods Sold", AccountType.Expense),
            (Account.SystemCodes.Purchases, "Purchases", AccountType.Expense),
            (Account.SystemCodes.PurchaseReturns, "Purchase Returns", AccountType.Expense),
            (Account.SystemCodes.Salaries, "Salaries Expense", AccountType.Expense),
            (Account.SystemCodes.Rent, "Rent Expense", AccountType.Expense),
            (Account.SystemCodes.Electricity, "Electricity Expense", AccountType.Expense),
            (Account.SystemCodes.MiscExpense, "Miscellaneous Expense", AccountType.Expense)
        };

        foreach (var (code, name, type) in accounts)
        {
            if (!codes.Contains(code))
            {
                _db.Accounts.Add(new Account { Code = code, Name = name, Type = type, IsSystem = true });
            }
        }

        await _db.SaveChangesAsync(ct);
    }
}
