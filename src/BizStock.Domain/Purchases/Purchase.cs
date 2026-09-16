using BizStock.Domain.Common;
using BizStock.Domain.Catalog;
using BizStock.Domain.Partners;

namespace BizStock.Domain.Purchases;

/// <summary>Purchase bill aggregate root.</summary>
public class Purchase : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique purchase number, e.g. PUR-000001.</summary>
    public string PurchaseNumber { get; set; } = default!;

    /// <summary>Supplier's invoice number.</summary>
    public string? SupplierInvoiceNumber { get; set; }

    /// <summary>Supplier identifier.</summary>
    public Guid SupplierId { get; set; }

    /// <summary>Supplier name snapshot.</summary>
    public string SupplierName { get; set; } = default!;

    /// <summary>Purchase date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Sum of line base amounts before discount and tax.</summary>
    public decimal SubTotal { get; set; }

    /// <summary>Bill-level discount.</summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>Total tax.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>CGST portion.</summary>
    public decimal CGST { get; set; }

    /// <summary>SGST portion.</summary>
    public decimal SGST { get; set; }

    /// <summary>IGST portion.</summary>
    public decimal IGST { get; set; }

    /// <summary>Other charges (freight, packing).</summary>
    public decimal OtherCharges { get; set; }

    /// <summary>Rounding adjustment.</summary>
    public decimal RoundOff { get; set; }

    /// <summary>Bill total.</summary>
    public decimal GrandTotal { get; set; }

    /// <summary>Paid at entry.</summary>
    public decimal PaidAmount { get; set; }

    /// <summary>Balance payable.</summary>
    public decimal BalanceAmount { get; set; }

    /// <summary>Payment method for the paid portion.</summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>Payment status.</summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid;

    /// <summary>Lifecycle status.</summary>
    public PurchaseStatus PurchaseStatus { get; set; } = PurchaseStatus.Completed;

    /// <summary>Note.</summary>
    public string? Notes { get; set; }

    /// <summary>Lines.</summary>
    public List<PurchaseItem> Items { get; set; } = [];

    /// <summary>Supplier navigation.</summary>
    public Supplier Supplier { get; set; } = default!;
}

/// <summary>Purchase line item.</summary>
public class PurchaseItem : BaseEntity
{
    /// <summary>Parent purchase id.</summary>
    public Guid PurchaseId { get; set; }

    /// <summary>Parent purchase navigation.</summary>
    public Purchase Purchase { get; set; } = default!;

    /// <summary>Product identifier.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Product name snapshot.</summary>
    public string ProductName { get; set; } = default!;

    /// <summary>Quantity received (positive).</summary>
    public decimal Quantity { get; set; }

    /// <summary>Unit purchase price (exclusive of tax).</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Line discount amount.</summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>Taxable amount after discount.</summary>
    public decimal TaxableAmount { get; set; }

    /// <summary>GST rate percent.</summary>
    public decimal GSTRate { get; set; }

    /// <summary>Line tax amount.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Line total incl. tax.</summary>
    public decimal LineTotal { get; set; }

    /// <summary>Optional batch number.</summary>
    public string? BatchNumber { get; set; }

    /// <summary>Optional expiry date.</summary>
    public DateTime? ExpiryDateUtc { get; set; }

    /// <summary>Free text.</summary>
    public string? Note { get; set; }

    /// <summary>Product navigation.</summary>
    public Product Product { get; set; } = default!;
}
