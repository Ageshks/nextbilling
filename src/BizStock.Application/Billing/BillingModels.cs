using BizStock.Domain.Common;

namespace BizStock.Application.Billing;

/// <summary>A line being billed at the counter.</summary>
public sealed class BillingLine
{
    /// <summary>Product id.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Product name snapshot.</summary>
    public string ProductName { get; set; } = default!;

    /// <summary>Quantity.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Unit price (exclusive of GST).</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Discount percent.</summary>
    public decimal DiscountPercent { get; set; }

    /// <summary>Flat discount amount (overrides percent when > 0).</summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>GST rate percent.</summary>
    public decimal GSTRate { get; set; }

    /// <summary>Unit cost for margin (assigned by the service).</summary>
    public decimal CostPrice { get; set; }

    /// <summary>Optional batch number.</summary>
    public string? BatchNumber { get; set; }
}

/// <summary>A new sale request.</summary>
public sealed class CreateSaleRequest
{
    /// <summary>Customer id, or null for walk-in.</summary>
    public Guid? CustomerId { get; set; }

    /// <summary>Customer name (required for walk-in or default).</summary>
    public string? CustomerName { get; set; }

    /// <summary>Invoice date (UTC). Null = now.</summary>
    public DateTime? DateUtc { get; set; }

    /// <summary>Document type. Default tax invoice.</summary>
    public InvoiceType InvoiceType { get; set; } = InvoiceType.TaxInvoice;

    /// <summary>Lines.</summary>
    public List<BillingLine> Lines { get; set; } = [];

    /// <summary>Invoice-level discount.</summary>
    public decimal InvoiceDiscount { get; set; }

    /// <summary>Amount collected now.</summary>
    public decimal PaidAmount { get; set; }

    /// <summary>Payment method for the collected amount.</summary>
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    /// <summary>Payment reference.</summary>
    public string? PaymentReference { get; set; }

    /// <summary>Salesperson.</summary>
    public string? SalesPerson { get; set; }

    /// <summary>Invoice note.</summary>
    public string? Notes { get; set; }
}

/// <summary>Totals of a computed bill (preview or saved).</summary>
public sealed record BillTotals(
    decimal SubTotal,
    decimal LineDiscounts,
    decimal InvoiceDiscount,
    decimal TaxableAmount,
    decimal CGST,
    decimal SGST,
    decimal IGST,
    decimal TotalTax,
    decimal GrandTotal,
    decimal RoundOff);

/// <summary>Saved sale summary returned after billing.</summary>
public sealed record SaleResult(
    Guid SaleId,
    string InvoiceNumber,
    BillTotals Totals,
    decimal PaidAmount,
    decimal BalanceAmount,
    PaymentStatus Status);

/// <summary>Result of processing a sale return.</summary>
public sealed record SalesReturnResult(Guid ReturnId, string ReturnNumber, decimal TotalAmount);
