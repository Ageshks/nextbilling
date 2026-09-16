using BizStock.Domain.Common;

namespace BizStock.Domain.Partners;

/// <summary>Direction of a partner payment.</summary>
public enum PartnerKind
{
    /// <summary>Money received from a customer (receipt).</summary>
    Customer = 1,

    /// <summary>Money paid to a supplier.</summary>
    Supplier = 2
}

/// <summary>
/// A receipt from a customer or a payment to a supplier. Every row is the auditable
/// source document behind a ledger entry and a balanced journal, so a receipt can be
/// reprinted and traced at any time.
/// </summary>
public class PartnerPayment : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique receipt/payment number, e.g. RCPT-2026-000007.</summary>
    public string PaymentNumber { get; set; } = default!;

    /// <summary>Whether this is a customer receipt or a supplier payment.</summary>
    public PartnerKind Kind { get; set; }

    /// <summary>Customer id when <see cref="Kind"/> is customer.</summary>
    public Guid? CustomerId { get; set; }

    /// <summary>Supplier id when <see cref="Kind"/> is supplier.</summary>
    public Guid? SupplierId { get; set; }

    /// <summary>Partner name snapshot.</summary>
    public string PartnerName { get; set; } = default!;

    /// <summary>Payment date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Amount settled.</summary>
    public decimal Amount { get; set; }

    /// <summary>Tender used.</summary>
    public PaymentMethod Method { get; set; }

    /// <summary>Cheque/UPI/UTR reference.</summary>
    public string? Reference { get; set; }

    /// <summary>Invoice this receipt is applied against, when known.</summary>
    public Guid? SaleId { get; set; }

    /// <summary>Purchase bill this payment is applied against, when known.</summary>
    public Guid? PurchaseId { get; set; }

    /// <summary>Optional narration.</summary>
    public string? Notes { get; set; }
}