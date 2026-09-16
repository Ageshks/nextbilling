using BizStock.Domain.Common;
using BizStock.Domain.Catalog;
using BizStock.Domain.Partners;
using BizStock.Domain.ValueObjects;

namespace BizStock.Domain.Sales;

/// <summary>
/// Sales invoice aggregate root. GST is computed with an exclusive split for
/// intra-state (CGST+SGST) versus inter-state (IGST) supplies.
/// </summary>
public class Sale : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique invoice number, e.g. INV-2026-000042.</summary>
    public string InvoiceNumber { get; set; } = default!;

    /// <summary>Invoice date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Document type (tax invoice, bill of supply, quotation…).</summary>
    public InvoiceType InvoiceType { get; set; } = InvoiceType.TaxInvoice;

    /// <summary>Customer identifier; null for walk-in counter sales.</summary>
    public Guid? CustomerId { get; set; }

    /// <summary>Customer name snapshot at billing time.</summary>
    public string CustomerName { get; set; } = default!;

    /// <summary>Customer GSTIN snapshot.</summary>
    public string? CustomerGSTIN { get; set; }

    /// <summary>Customer state code snapshot; drives intra vs inter-state tax split.</summary>
    public string? CustomerStateCode { get; set; }

    /// <summary>Business home state code snapshot for the split decision.</summary>
    public string? BusinessStateCode { get; set; }

    /// <summary>Sum of line base amounts before discount and tax.</summary>
    public decimal SubTotal { get; set; }

    /// <summary>Invoice-level discount amount (part of DiscountAmount).</summary>
    public decimal InvoiceDiscount { get; set; }

    /// <summary>Discount amount (line + invoice level).</summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>Taxable amount after all discounts.</summary>
    public decimal TaxableAmount { get; set; }

    /// <summary>Total tax (CGST+SGST or IGST).</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>CGST total (intra-state only).</summary>
    public decimal CGST { get; set; }

    /// <summary>SGST total (intra-state only).</summary>
    public decimal SGST { get; set; }

    /// <summary>IGST total (inter-state only).</summary>
    public decimal IGST { get; set; }

    /// <summary>Cess total.</summary>
    public decimal Cess { get; set; }

    /// <summary>Rounding adjustment applied to the payable amount.</summary>
    public decimal RoundOff { get; set; }

    /// <summary>Final payable amount.</summary>
    public decimal GrandTotal { get; set; }

    /// <summary>Amount settled at billing time.</summary>
    public decimal PaidAmount { get; set; }

    /// <summary>Outstanding amount (GrandTotal - PaidAmount - returned).</summary>
    public decimal BalanceAmount { get; set; }

    /// <summary>Payment method used at the counter.</summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>Payment status.</summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Paid;

    /// <summary>Lifecycle status.</summary>
    public SaleStatus SaleStatus { get; set; } = SaleStatus.Completed;

    /// <summary>Salesperson username.</summary>
    public string? SalesPerson { get; set; }

    /// <summary>Invoice note.</summary>
    public string? Notes { get; set; }

    /// <summary>Lines.</summary>
    public List<SaleItem> Items { get; set; } = [];

    /// <summary>Payments applied.</summary>
    public List<SalePayment> Payments { get; set; } = [];

    /// <summary>Returns raised against this invoice.</summary>
    public List<SalesReturn> Returns { get; set; } = [];

    /// <summary>Whether this sale is intra-state (CGST+SGST) versus inter-state (IGST).</summary>
    /// <remarks>
    /// Default when either state code is unknown is intra-state, matching the
    /// common small-retailer case where customer state is the home state.
    /// </remarks>
    public bool IsIntraState =>
        string.IsNullOrEmpty(CustomerStateCode) ||
        string.IsNullOrEmpty(BusinessStateCode) ||
        CustomerStateCode == BusinessStateCode;

    /// <summary>Computes GST components from a taxable amount and rate.</summary>
    public static (decimal Tax, decimal CGST, decimal SGST, decimal IGST) ComputeGST(decimal taxable, decimal rate, bool intraState)
    {
        var tax = Money.Round(taxable * rate / 100m);
        return intraState
            ? (tax, Money.Round(tax / 2m), Money.Round(tax / 2m), 0m)
            : (tax, 0m, 0m, tax);
    }
}

/// <summary>Sale line item with line-level discount and GST.</summary>
public class SaleItem : BaseEntity
{
    /// <summary>Parent sale id.</summary>
    public Guid SaleId { get; set; }

    /// <summary>Product identifier.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Product name snapshot for historical integrity.</summary>
    public string ProductName { get; set; } = default!;

    /// <summary>Quantity sold (positive).</summary>
    public decimal Quantity { get; set; }

    /// <summary>Unit price before discount and tax (exclusive).</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Discount percent applied to the line.</summary>
    public decimal DiscountPercent { get; set; }

    /// <summary>Discount amount applied to the line.</summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>Taxable amount after discount (SubTotal - Discount).</summary>
    public decimal TaxableAmount { get; set; }

    /// <summary>GST rate percent for the line.</summary>
    public decimal GSTRate { get; set; }

    /// <summary>Line GST amount.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Line total including tax.</summary>
    public decimal LineTotal { get; set; }

    /// <summary>Unit cost at sale time for margin analysis.</summary>
    public decimal CostPrice { get; set; }

    /// <summary>Optional batch number.</summary>
    public string? BatchNumber { get; set; }

    /// <summary>Product navigation.</summary>
    public Product Product { get; set; } = default!;

    /// <summary>Computes the taxable amount for this line: quantity × price − discount.</summary>
    public decimal ComputeTaxable() => Money.Round((Quantity * UnitPrice) - DiscountAmount, 2);
}
