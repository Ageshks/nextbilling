using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Common;
using BizStock.Domain.Inventory;
using BizStock.Domain.Partners;
using BizStock.Domain.Sales;
using BizStock.Domain.ValueObjects;

namespace BizStock.Application.Billing;

/// <summary>Sale construction helpers.</summary>
public partial class BillingService
{
    private async Task<Sale> BuildSaleAsync(CreateSaleRequest request, Customer? customer,
        Domain.Business.Business? business, CancellationToken ct)
    {
        return new Sale
        {
            BusinessId = business?.Id,
            InvoiceNumber = await _sequences.GetNextNumberAsync(DocumentType.SaleInvoice, ct),
            DateUtc = request.DateUtc ?? DateTime.UtcNow,
            InvoiceType = request.InvoiceType,
            CustomerId = customer?.Id,
            CustomerName = customer?.Name ?? request.CustomerName ?? "Walk-in Customer",
            CustomerGSTIN = customer?.GSTIN,
            CustomerStateCode = customer?.StateCode ?? business?.StateCode,
            BusinessStateCode = business?.StateCode,
            InvoiceDiscount = Money.Round(request.InvoiceDiscount),
            SalesPerson = request.SalesPerson,
            Notes = request.Notes
        };
    }

    private async Task AddSaleLineAsync(Sale sale, BillingLine line, bool intraState, CancellationToken ct)
    {
        var product = await _db.Products.FindAsync([line.ProductId], ct)
            ?? throw new BusinessRuleViolationException($"Product '{line.ProductName}' not found.");
        if (!product.IsActive)
        {
            throw new BusinessRuleViolationException($"Product '{product.Name}' is inactive.");
        }

        var gross = Money.Round(line.Quantity * line.UnitPrice);
        var discount = GetLineDiscount(line, gross);
        var taxable = Money.Round(gross - discount);
        var (tax, c, s, i) = Sale.ComputeGST(taxable, product.GSTRate, intraState);

        sale.Items.Add(new SaleItem
        {
            SaleId = sale.Id,
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
            DiscountPercent = line.DiscountPercent,
            DiscountAmount = discount,
            TaxableAmount = taxable,
            GSTRate = product.GSTRate,
            TaxAmount = tax,
            LineTotal = Money.Round(taxable + tax),
            CostPrice = product.PurchasePrice,
            BatchNumber = line.BatchNumber
        });

        sale.SubTotal = Money.Round(sale.SubTotal + gross);
        sale.DiscountAmount = Money.Round(sale.DiscountAmount + discount);
        sale.TaxAmount = Money.Round(sale.TaxAmount + tax);
        sale.CGST = Money.Round(sale.CGST + c);
        sale.SGST = Money.Round(sale.SGST + s);
        sale.IGST = Money.Round(sale.IGST + i);

        await _stock.ApplyMovementAsync(
            product.Id, StockMovementType.Sale, -line.Quantity, product.PurchasePrice,
            sale.InvoiceNumber, sale.Id, line.BatchNumber, null, sale.DateUtc, ct);
    }

    private static void FinalizeSale(Sale sale, bool roundOffEnabled, CreateSaleRequest request)
    {
        sale.DiscountAmount = Money.Round(sale.DiscountAmount + sale.InvoiceDiscount);
        sale.SubTotal = Money.Round(sale.SubTotal);
        sale.TaxableAmount = Money.Round(sale.SubTotal - sale.DiscountAmount);
        sale.TaxAmount = Money.Round(sale.TaxAmount);
        var grand = Money.Round(sale.TaxableAmount + sale.TaxAmount);
        if (roundOffEnabled)
        {
            var rounded = Math.Round(grand, MidpointRounding.AwayFromZero);
            sale.RoundOff = Money.Round(rounded - grand);
            grand = rounded;
        }
        sale.GrandTotal = grand;

        sale.PaidAmount = Money.Round(Math.Min(request.PaidAmount, grand));
        sale.BalanceAmount = Money.Round(grand - sale.PaidAmount);
        sale.Status = sale.BalanceAmount switch
        {
            <= 0 => PaymentStatus.Paid,
            _ when sale.PaidAmount > 0 => PaymentStatus.Partial,
            _ => PaymentStatus.Unpaid
        };
        sale.PaymentMethod = request.PaymentMethod;

        if (sale.PaidAmount > 0)
        {
            sale.Payments.Add(new SalePayment
            {
                SaleId = sale.Id,
                DateUtc = sale.DateUtc,
                Amount = sale.PaidAmount,
                Method = request.PaymentMethod,
                Reference = request.PaymentReference
            });
        }
    }
}
