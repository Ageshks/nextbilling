using BizStock.Domain.Common;
using BizStock.Domain.ValueObjects;

namespace BizStock.Domain.Finance;

/// <summary>Chart-of-accounts account.</summary>
public class Account : BaseEntity, IActivatable
{
    /// <summary>Unique account code, e.g. 1000.</summary>
    public string Code { get; set; } = default!;

    /// <summary>Account name.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Account classification.</summary>
    public AccountType Type { get; set; }

    /// <summary>Whether this is a seeded system account that cannot be deleted.</summary>
    public bool IsSystem { get; set; }

    /// <summary>Whether the account can be used.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Well-known system account codes.</summary>
    public static class SystemCodes
    {
        /// <summary>Cash in hand.</summary>
        public const string Cash = "1000";

        /// <summary>Bank account.</summary>
        public const string Bank = "1010";

        /// <summary>Accounts receivable.</summary>
        public const string AccountsReceivable = "1100";

        /// <summary>Inventory.</summary>
        public const string Inventory = "1200";

        /// <summary>Input GST credit.</summary>
        public const string InputGST = "1300";

        /// <summary>Accounts payable.</summary>
        public const string AccountsPayable = "2000";

        /// <summary>Output GST payable.</summary>
        public const string OutputGST = "2100";

        /// <summary>Owner capital.</summary>
        public const string OwnerCapital = "3000";

        /// <summary>Owner drawings.</summary>
        public const string OwnerDrawings = "3100";

        /// <summary>Retained earnings.</summary>
        public const string RetainedEarnings = "3200";

        /// <summary>Sales revenue.</summary>
        public const string Sales = "4000";

        /// <summary>Sales returns.</summary>
        public const string SalesReturns = "4100";

        /// <summary>Other income.</summary>
        public const string OtherIncome = "4200";

        /// <summary>Cost of goods sold.</summary>
        public const string CostOfGoodsSold = "5000";

        /// <summary>Purchases.</summary>
        public const string Purchases = "5100";

        /// <summary>Purchase returns.</summary>
        public const string PurchaseReturns = "5200";

        /// <summary>Salaries expense.</summary>
        public const string Salaries = "6000";

        /// <summary>Rent expense.</summary>
        public const string Rent = "6010";

        /// <summary>Electricity expense.</summary>
        public const string Electricity = "6020";

        /// <summary>General/misc expense.</summary>
        public const string MiscExpense = "6900";
    }
}

/// <summary>Journal voucher header. Posting requires debits == credits.</summary>
public class JournalEntry : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique voucher number, e.g. JV-000001.</summary>
    public string VoucherNumber { get; set; } = default!;

    /// <summary>Voucher date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Voucher category.</summary>
    public VoucherType VoucherType { get; set; }

    /// <summary>Reference document number.</summary>
    public string? Reference { get; set; }

    /// <summary>Source document id.</summary>
    public Guid? DocumentId { get; set; }

    /// <summary>Narration.</summary>
    public string? Narration { get; set; }

    /// <summary>Draft / posted.</summary>
    public JournalStatus Status { get; set; } = JournalStatus.Posted;

    /// <summary>Lines.</summary>
    public List<JournalLine> Lines { get; set; } = [];

    /// <summary>Total debit amount across lines.</summary>
    public decimal TotalDebit => Money.Round(Lines.Sum(l => l.Debit));

    /// <summary>Total credit amount across lines.</summary>
    public decimal TotalCredit => Money.Round(Lines.Sum(l => l.Credit));
}

/// <summary>Journal line: debit or credit against one account.</summary>
public class JournalLine : BaseEntity
{
    /// <summary>Parent journal id.</summary>
    public Guid JournalEntryId { get; set; }

    /// <summary>Parent journal navigation.</summary>
    public JournalEntry JournalEntry { get; set; } = default!;

    /// <summary>Account id.</summary>
    public Guid AccountId { get; set; }

    /// <summary>Debit amount (0 for credit lines).</summary>
    public decimal Debit { get; set; }

    /// <summary>Credit amount (0 for debit lines).</summary>
    public decimal Credit { get; set; }

    /// <summary>Line narration.</summary>
    public string? Narration { get; set; }

    /// <summary>Account navigation.</summary>
    public Account Account { get; set; } = default!;
}
