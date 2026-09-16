using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Common;
using BizStock.Domain.Finance;
using BizStock.Domain.Sales;

namespace BizStock.Application.Billing;

/// <summary>Accounting postings for completed sales.</summary>
public partial class BillingService
{
    /// <summary>
    /// Posts the double-entry journal for a sale:
    /// debit cash/bank (paid portion) and accounts receivable (balance),
    /// credit sales revenue and output GST.
    /// </summary>
    private async Task PostSaleJournalAsync(Sale sale, CreateSaleRequest request, CancellationToken ct)
    {
        var debitAccount = sale.PaidAmount <= 0
            ? Account.SystemCodes.AccountsReceivable
            : request.PaymentMethod switch
            {
                PaymentMethod.BankTransfer or PaymentMethod.Card or PaymentMethod.Upi => Account.SystemCodes.Bank,
                _ => Account.SystemCodes.Cash
            };

        var debitId = await _accounting.GetSystemAccountIdAsync(debitAccount, ct);
        var salesId = await _accounting.GetSystemAccountIdAsync(Account.SystemCodes.Sales, ct);

        if (sale.TaxableAmount > 0)
        {
            await _accounting.PostSimpleAsync(sale.DateUtc, VoucherType.Sale, sale.InvoiceNumber,
                debitId, salesId, sale.TaxableAmount, $"Sales revenue {sale.InvoiceNumber}", sale.Id, ct);
        }

        if (sale.TaxAmount > 0)
        {
            await _accounting.PostSimpleAsync(sale.DateUtc, VoucherType.Sale, sale.InvoiceNumber,
                debitId,
                await _accounting.GetSystemAccountIdAsync(Account.SystemCodes.OutputGST, ct),
                sale.TaxAmount, $"GST on {sale.InvoiceNumber}", sale.Id, ct);
        }
    }
}
