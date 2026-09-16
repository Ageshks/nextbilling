using BizStock.Domain.Catalog;
using BizStock.Domain.Common;

namespace BizStock.Domain.Purchases;

/// <summary>Purchase return (debit note) header. Reduces stock and payable.</summary>
public class PurchaseReturn : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique return number, e.g. PR-000001.</summary>
    public string ReturnNumber { get; set; } = default!;

    /// <summary>Original purchase id.</summary>
    public Guid PurchaseId { get; set; }

    /// <summary>Original purchase number snapshot.</summary>
    public string PurchaseNumber { get; set; } = default!;

    /// <summary>Supplier id.</summary>
    public Guid SupplierId { get; set; }

    /// <summary>Supplier name snapshot.</summary>
    public string SupplierName { get; set; } = default!;

    /// <summary>Return date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Sum of returned line taxable amounts.</summary>
    public decimal SubTotal { get; set; }

    /// <summary>Tax portion.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Total credit amount.</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Reason.</summary>
    public string? Reason { get; set; }

    /// <summary>Settlement method.</summary>
    public PaymentMethod SettlementMethod { get; set; }

    /// <summary>Lines.</summary>
    public List<PurchaseReturnItem> Items { get; set; } = [];

    /// <summary>Purchase navigation.</summary>
    public Purchase Purchase { get; set; } = default!;
}

/// <summary>Purchase return line.</summary>
public class PurchaseReturnItem : BaseEntity
{
    /// <summary>Parent return id.</summary>
    public Guid PurchaseReturnId { get; set; }

    /// <summary>Original purchase item id.</summary>
    public Guid PurchaseItemId { get; set; }

    /// <summary>Product id.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Product name snapshot.</summary>
    public string ProductName { get; set; } = default!;

    /// <summary>Quantity returned.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Unit price at return.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>GST rate percent.</summary>
    public decimal GSTRate { get; set; }

    /// <summary>Tax for returned qty.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Line total incl. tax.</summary>
    public decimal LineTotal { get; set; }

    /// <summary>Product navigation.</summary>
    public Product Product { get; set; } = default!;
}
