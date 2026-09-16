using BizStock.Domain.Common;
using BizStock.Domain.Catalog;
using BizStock.Domain.Partners;

namespace BizStock.Domain.Sales;

/// <summary>Payment applied against a sale (full or part).</summary>
public class SalePayment : BaseEntity
{
    /// <summary>Sale identifier.</summary>
    public Guid SaleId { get; set; }

    /// <summary>Payment date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Amount paid.</summary>
    public decimal Amount { get; set; }

    /// <summary>Payment method.</summary>
    public PaymentMethod Method { get; set; }

    /// <summary>Reference (UPI txn id, cheque no…).</summary>
    public string? Reference { get; set; }

    /// <summary>Note.</summary>
    public string? Note { get; set; }
}

/// <summary>Sales return (credit note) header. Restores stock and adjusts customer balance.</summary>
public class SalesReturn : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique return number, e.g. SR-000001.</summary>
    public string ReturnNumber { get; set; } = default!;

    /// <summary>Original sale identifier.</summary>
    public Guid SaleId { get; set; }

    /// <summary>Original invoice number snapshot.</summary>
    public string InvoiceNumber { get; set; } = default!;

    /// <summary>Customer identifier.</summary>
    public Guid? CustomerId { get; set; }

    /// <summary>Customer name snapshot.</summary>
    public string CustomerName { get; set; } = default!;

    /// <summary>Return date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Sum of returned line taxable amounts.</summary>
    public decimal SubTotal { get; set; }

    /// <summary>Tax refunded with the return.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Total refundable amount.</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Reason.</summary>
    public string? Reason { get; set; }

    /// <summary>How the refund was settled.</summary>
    public PaymentMethod RefundMethod { get; set; }

    /// <summary>Lines.</summary>
    public List<SalesReturnItem> Items { get; set; } = [];

    /// <summary>Sale navigation.</summary>
    public Sale Sale { get; set; } = default!;
}

/// <summary>Sales return line.</summary>
public class SalesReturnItem : BaseEntity
{
    /// <summary>Parent return id.</summary>
    public Guid SalesReturnId { get; set; }

    /// <summary>Original sale item id.</summary>
    public Guid SaleItemId { get; set; }

    /// <summary>Product identifier.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Product name snapshot.</summary>
    public string ProductName { get; set; } = default!;

    /// <summary>Quantity returned (positive).</summary>
    public decimal Quantity { get; set; }

    /// <summary>Unit price at return.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>GST rate percent.</summary>
    public decimal GSTRate { get; set; }

    /// <summary>Tax amount for the returned quantity.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Line total (incl. tax).</summary>
    public decimal LineTotal { get; set; }

    /// <summary>Product navigation.</summary>
    public Product Product { get; set; } = default!;
}

/// <summary>Held/parked bill. Kept for recall on the billing screen.</summary>
public class HeldSale : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique hold number, e.g. HOLD-000001.</summary>
    public string HoldNumber { get; set; } = default!;

    /// <summary>Customer identifier.</summary>
    public Guid? CustomerId { get; set; }

    /// <summary>Customer name snapshot.</summary>
    public string CustomerName { get; set; } = default!;

    /// <summary>When the bill was held (UTC).</summary>
    public DateTime HeldAtUtc { get; set; }

    /// <summary>Serialized cart lines (JSON).</summary>
    public string CartJson { get; set; } = default!;

    /// <summary>Note.</summary>
    public string? Note { get; set; }
}
