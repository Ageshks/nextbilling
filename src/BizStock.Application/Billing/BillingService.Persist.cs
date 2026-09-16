using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Common;
using BizStock.Domain.Inventory;
using BizStock.Domain.Partners;
using BizStock.Domain.Sales;

namespace BizStock.Application.Billing;

/// <summary>Persistence side of the billing engine.</summary>
public partial class BillingService
{
    /// <summary>Creates and persists a sale. Validates stock and balances the books atomically.</summary>
    public async Task<SaleResult> CreateSaleAsync(CreateSaleRequest request, CancellationToken ct = default)
    {
        if (request.Lines.Count == 0)
        {
            throw new BusinessRuleViolationException("A sale requires at least one item.");
        }

        var settings = await _business.GetSettingsAsync(ct);
        var business = await _business.GetBusinessAsync(ct);
        var intraState = string.IsNullOrEmpty(business?.StateCode);

        Customer? customer = null;
        if (request.CustomerId is Guid cid)
        {
            customer = await _db.Customers.FindAsync([cid], ct)
                ?? throw new BusinessRuleViolationException("Customer not found.");
            if (!customer.IsActive)
            {
                throw new BusinessRuleViolationException($"Customer '{customer.Name}' is inactive.");
            }
        }

        return await _uow.ExecuteInTransactionAsync(async innerCt =>
        {
            var sale = await BuildSaleAsync(request, customer, business, innerCt);

            foreach (var line in request.Lines)
            {
                await AddSaleLineAsync(sale, line, intraState, innerCt);
            }

            FinalizeSale(sale, settings.RoundOffEnabled, request);

            _db.Sales.Add(sale);

            if (customer is not null)
            {
                AddCustomerLedger(sale, customer);
            }

            await _db.SaveChangesAsync(innerCt);

            await PostSaleJournalAsync(sale, request, innerCt);

            await _audit.RecordAsync("SaleCreated", "Billing", nameof(Sale), sale.Id.ToString(),
                null, new { sale.InvoiceNumber, sale.GrandTotal, Items = sale.Items.Count }, innerCt);

            return new SaleResult(
                sale.Id,
                sale.InvoiceNumber,
                new BillTotals(sale.SubTotal, sale.DiscountAmount - sale.InvoiceDiscount, sale.InvoiceDiscount,
                    sale.TaxableAmount, sale.CGST, sale.SGST, sale.IGST, sale.TaxAmount, sale.GrandTotal, sale.RoundOff),
                sale.PaidAmount,
                sale.BalanceAmount,
                sale.Status);
        }, ct);
    }
}
