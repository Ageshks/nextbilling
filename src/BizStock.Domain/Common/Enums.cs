namespace BizStock.Domain.Common;

/// <summary>Methods of payment supported at billing/receipt time.</summary>
public enum PaymentMethod
{
    /// <summary>Cash.</summary>
    Cash = 1,

    /// <summary>UPI (mobile payment).</summary>
    Upi = 2,

    /// <summary>Debit/Credit card.</summary>
    Card = 3,

    /// <summary>Bank transfer / NEFT / IMPS.</summary>
    BankTransfer = 4,

    /// <summary>Cheque.</summary>
    Cheque = 5,

    /// <summary>Sold on credit (no immediate payment).</summary>
    Credit = 6
}

/// <summary>Invoice payment status.</summary>
public enum PaymentStatus
{
    /// <summary>Nothing paid yet.</summary>
    Unpaid = 0,

    /// <summary>Partially paid.</summary>
    Partial = 1,

    /// <summary>Fully paid.</summary>
    Paid = 2
}

/// <summary>Accounting voucher categories.</summary>
public enum VoucherType
{
    /// <summary>Sales invoice.</summary>
    Sale = 1,

    /// <summary>Purchase bill.</summary>
    Purchase = 2,

    /// <summary>Sales return (credit note).</summary>
    SalesReturn = 3,

    /// <summary>Purchase return (debit note).</summary>
    PurchaseReturn = 4,

    /// <summary>Customer receipt / payment.</summary>
    CustomerReceipt = 5,

    /// <summary>Supplier payment.</summary>
    SupplierPayment = 6,

    /// <summary>Expense.</summary>
    Expense = 7,

    /// <summary>Other income.</summary>
    Income = 8,

    /// <summary>Adjustment journal.</summary>
    Journal = 9,

    /// <summary>Opening balances.</summary>
    Opening = 10,

    /// <summary>Stock adjustment.</summary>
    Adjustment = 11
}

/// <summary>Chart-of-accounts account classification.</summary>
public enum AccountType
{
    /// <summary>Assets.</summary>
    Asset = 1,

    /// <summary>Liabilities.</summary>
    Liability = 2,

    /// <summary>Equity.</summary>
    Equity = 3,

    /// <summary>Income.</summary>
    Income = 4,

    /// <summary>Expenses.</summary>
    Expense = 5
}

/// <summary>Stock movement categories for the stock ledger.</summary>
public enum StockMovementType
{
    /// <summary>Opening stock.</summary>
    Opening = 1,

    /// <summary>Purchase receipt.</summary>
    Purchase = 2,

    /// <summary>Sale issue.</summary>
    Sale = 3,

    /// <summary>Sales return in.</summary>
    SalesReturn = 4,

    /// <summary>Purchase return out.</summary>
    PurchaseReturn = 5,

    /// <summary>Manual adjustment.</summary>
    Adjustment = 6,

    /// <summary>Damage write-off.</summary>
    Damage = 7,

    /// <summary>Expiry write-off.</summary>
    Expiry = 8,

    /// <summary>Transfer in.</summary>
    TransferIn = 9,

    /// <summary>Transfer out.</summary>
    TransferOut = 10
}

/// <summary>Lifecycle status of a sales invoice.</summary>
public enum SaleStatus
{
    /// <summary>Completed sale.</summary>
    Completed = 1,

    /// <summary>Cancelled sale.</summary>
    Cancelled = 2
}

/// <summary>Kinds of documents that can be issued at billing.</summary>
public enum InvoiceType
{
    /// <summary>Regular GST tax invoice.</summary>
    TaxInvoice = 1,

    /// <summary>Bill of supply (no tax charged, e.g. composition dealer).</summary>
    BillOfSupply = 2,

    /// <summary>Quotation.</summary>
    Quotation = 3,

    /// <summary>Proforma invoice.</summary>
    ProformaInvoice = 4,

    /// <summary>Credit note.</summary>
    CreditNote = 5,

    /// <summary>Debit note.</summary>
    DebitNote = 6
}

/// <summary>Lifecycle status of a purchase bill.</summary>
public enum PurchaseStatus
{
    /// <summary>Completed purchase.</summary>
    Completed = 1,

    /// <summary>Cancelled purchase.</summary>
    Cancelled = 2
}

/// <summary>Reasons for stock adjustments.</summary>
public enum AdjustmentReason
{
    /// <summary>Physical re-count correction.</summary>
    ReCount = 1,

    /// <summary>Damaged goods.</summary>
    Damage = 2,

    /// <summary>Expired goods.</summary>
    Expiry = 3,

    /// <summary>Theft/loss.</summary>
    Theft = 4,

    /// <summary>Data correction.</summary>
    Correction = 5,

    /// <summary>Other.</summary>
    Other = 6
}

/// <summary>Status of an adjustment document.</summary>
public enum AdjustmentStatus
{
    /// <summary>Posted (stock applied).</summary>
    Posted = 1,

    /// <summary>Cancelled (stock reversed).</summary>
    Cancelled = 2
}

/// <summary>Document types for number sequences.</summary>
public enum DocumentType
{
    /// <summary>Sales invoice number.</summary>
    SaleInvoice = 1,

    /// <summary>Purchase bill number.</summary>
    Purchase = 2,

    /// <summary>Sales return number.</summary>
    SalesReturn = 3,

    /// <summary>Purchase return number.</summary>
    PurchaseReturn = 4,

    /// <summary>Stock adjustment number.</summary>
    StockAdjustment = 5,

    /// <summary>Journal voucher number.</summary>
    JournalEntry = 6,

    /// <summary>Customer ledger receipt number.</summary>
    CustomerReceipt = 7,

    /// <summary>Supplier ledger payment number.</summary>
    SupplierPayment = 8,

    /// <summary>Customer code.</summary>
    Customer = 9,

    /// <summary>Supplier code.</summary>
    Supplier = 10,

    /// <summary>Product code.</summary>
    Product = 11,

    /// <summary>Held bill number.</summary>
    HeldSale = 12
}

/// <summary>Status of a journal entry.</summary>
public enum JournalStatus
{
    /// <summary>Draft (not yet posted).</summary>
    Draft = 0,

    /// <summary>Posted to the ledger.</summary>
    Posted = 1
}

/// <summary>Kind of database backup.</summary>
public enum BackupKind
{
    /// <summary>Manual user-triggered backup.</summary>
    Manual = 1,

    /// <summary>Automatic scheduled backup.</summary>
    Scheduled = 2,

    /// <summary>Safety backup taken before a restore.</summary>
    PreRestore = 3
}
