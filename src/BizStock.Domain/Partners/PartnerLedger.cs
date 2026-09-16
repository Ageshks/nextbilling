using BizStock.Domain.Common;
using BizStock.Domain.Partners;

namespace BizStock.Domain.Partners;

/// <summary>Customer-side ledger entry (statement of account). Debit increases receivable.</summary>
public class CustomerLedger : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Customer identifier.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Posting date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Voucher type of the source document.</summary>
    public VoucherType VoucherType { get; set; }

    /// <summary>Reference document number.</summary>
    public string Reference { get; set; } = default!;

    /// <summary>Source document id.</summary>
    public Guid? DocumentId { get; set; }

    /// <summary>Debit (increases what customer owes).</summary>
    public decimal Debit { get; set; }

    /// <summary>Credit (reduces what customer owes).</summary>
    public decimal Credit { get; set; }

    /// <summary>Free-text description.</summary>
    public string? Description { get; set; }

    /// <summary>Customer navigation.</summary>
    public Customer Customer { get; set; } = default!;
}

/// <summary>Supplier-side ledger entry. Credit increases payable.</summary>
public class SupplierLedger : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Supplier identifier.</summary>
    public Guid SupplierId { get; set; }

    /// <summary>Posting date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Voucher type of the source document.</summary>
    public VoucherType VoucherType { get; set; }

    /// <summary>Reference document number.</summary>
    public string Reference { get; set; } = default!;

    /// <summary>Source document id.</summary>
    public Guid? DocumentId { get; set; }

    /// <summary>Debit (reduces what we owe).</summary>
    public decimal Debit { get; set; }

    /// <summary>Credit (increases what we owe).</summary>
    public decimal Credit { get; set; }

    /// <summary>Free-text description.</summary>
    public string? Description { get; set; }

    /// <summary>Supplier navigation.</summary>
    public Supplier Supplier { get; set; } = default!;
}
