using BizStock.Domain.Common;
using BizStock.Domain.Partners;
using BizStock.Domain.Sales;
using BizStock.Domain.ValueObjects;

namespace BizStock.Application.Billing;

/// <summary>Customer ledger application.</summary>
public partial class BillingService
{
    private void AddCustomerLedger(Sale sale, Customer customer)
    {
        _db.CustomerLedgers.Add(new CustomerLedger
        {
            BusinessId = sale.BusinessId,
            CustomerId = customer.Id,
            DateUtc = sale.DateUtc,
            VoucherType = VoucherType.Sale,
            Reference = sale.InvoiceNumber,
            DocumentId = sale.Id,
            Debit = sale.GrandTotal,
            Credit = 0,
            Description = $"Invoice {sale.InvoiceNumber}"
        });
        customer.OutstandingBalance = Money.Round(customer.OutstandingBalance + sale.GrandTotal);
    }
}
