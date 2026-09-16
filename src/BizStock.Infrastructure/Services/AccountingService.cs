using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Common;
using BizStock.Domain.Finance;
using BizStock.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

using BizStock.Infrastructure.Persistence;
namespace BizStock.Infrastructure.Services;

/// <summary>
/// Double-entry posting engine. Validates debit == credit before persisting,
/// guaranteeing the books always balance.
/// </summary>
public class AccountingService : IAccountingService
{
    private readonly AppDbContext _db;
    private readonly INumberSequenceService _sequences;
    private readonly IUnitOfWork _uow;

    /// <summary>Creates the accounting service.</summary>
    public AccountingService(AppDbContext db, INumberSequenceService sequences, IUnitOfWork uow)
    {
        _db = db;
        _sequences = sequences;
        _uow = uow;
    }

    /// <inheritdoc />
    public async Task<Guid> PostAsync(JournalEntry entry, CancellationToken ct = default)
    {
        if (entry.Lines.Count < 2)
        {
            throw new BusinessRuleViolationException("A journal entry requires at least two lines.");
        }

        var debit = Money.Round(entry.Lines.Sum(l => l.Debit));
        var credit = Money.Round(entry.Lines.Sum(l => l.Credit));
        if (debit != credit || debit <= 0)
        {
            throw new UnbalancedJournalException(debit, credit);
        }

        foreach (var line in entry.Lines)
        {
            if (line.Debit > 0 && line.Credit > 0)
            {
                throw new BusinessRuleViolationException("A journal line must be either debit or credit, not both.");
            }
        }

        entry.VoucherNumber = await _sequences.GetNextNumberAsync(DocumentType.JournalEntry, ct);
        entry.Status = JournalStatus.Posted;

        _db.JournalEntries.Add(entry);
        await _db.SaveChangesAsync(ct);
        return entry.Id;
    }

    /// <inheritdoc />
    public Task<Guid> PostSimpleAsync(
        DateTime dateUtc, VoucherType type, string reference,
        Guid debitAccountId, Guid creditAccountId, decimal amount,
        string? narration = null, Guid? documentId = null, CancellationToken ct = default)
    {
        if (amount <= 0)
        {
            throw new BusinessRuleViolationException("Posting amount must be positive.");
        }

        var entry = new JournalEntry
        {
            DateUtc = dateUtc,
            VoucherType = type,
            Reference = reference,
            DocumentId = documentId,
            Narration = narration,
            Lines =
            [
                new JournalLine { AccountId = debitAccountId, Debit = Money.Round(amount), Credit = 0, Narration = narration },
                new JournalLine { AccountId = creditAccountId, Debit = 0, Credit = Money.Round(amount), Narration = narration }
            ]
        };

        return PostAsync(entry, ct);
    }

    /// <inheritdoc />
    public async Task<Guid> GetSystemAccountIdAsync(string code, CancellationToken ct = default)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Code == code, ct);
        if (account is not null)
        {
            return account.Id;
        }

        account = new Account
        {
            Code = code,
            Name = DefaultNameFor(code),
            Type = DefaultTypeFor(code),
            IsSystem = true
        };
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync(ct);
        return account.Id;
    }

    private static string DefaultNameFor(string code) => code switch
    {
        Account.SystemCodes.Cash => "Cash in Hand",
        Account.SystemCodes.Bank => "Bank Account",
        Account.SystemCodes.AccountsReceivable => "Accounts Receivable",
        Account.SystemCodes.Inventory => "Inventory",
        Account.SystemCodes.InputGST => "Input GST Credit",
        Account.SystemCodes.AccountsPayable => "Accounts Payable",
        Account.SystemCodes.OutputGST => "Output GST Payable",
        Account.SystemCodes.OwnerCapital => "Owner's Capital",
        Account.SystemCodes.OwnerDrawings => "Owner's Drawings",
        Account.SystemCodes.RetainedEarnings => "Retained Earnings",
        Account.SystemCodes.Sales => "Sales Revenue",
        Account.SystemCodes.SalesReturns => "Sales Returns",
        Account.SystemCodes.OtherIncome => "Other Income",
        Account.SystemCodes.CostOfGoodsSold => "Cost of Goods Sold",
        Account.SystemCodes.Purchases => "Purchases",
        Account.SystemCodes.PurchaseReturns => "Purchase Returns",
        Account.SystemCodes.Salaries => "Salaries Expense",
        Account.SystemCodes.Rent => "Rent Expense",
        Account.SystemCodes.Electricity => "Electricity Expense",
        Account.SystemCodes.MiscExpense => "Miscellaneous Expense",
        _ => $"Account {code}"
    };

    private static AccountType DefaultTypeFor(string code) => code switch
    {
        Account.SystemCodes.Cash or Account.SystemCodes.Bank or Account.SystemCodes.AccountsReceivable
            or Account.SystemCodes.Inventory or Account.SystemCodes.InputGST => AccountType.Asset,
        Account.SystemCodes.AccountsPayable or Account.SystemCodes.OutputGST => AccountType.Liability,
        Account.SystemCodes.OwnerCapital or Account.SystemCodes.RetainedEarnings => AccountType.Equity,
        Account.SystemCodes.OwnerDrawings => AccountType.Asset, // contra-equity tracked as debit balance
        Account.SystemCodes.Sales or Account.SystemCodes.OtherIncome => AccountType.Income,
        _ => AccountType.Expense
    };
}
