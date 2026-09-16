using BizStock.Domain.Common;

namespace BizStock.Domain.Finance;

/// <summary>
/// Non-sales income (interest, scrap sale, commission received…). Posting an income
/// record always writes a balanced journal: debit cash/bank, credit the income account.
/// </summary>
public class Income : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique income number, e.g. INC-000001.</summary>
    public string IncomeNumber { get; set; } = default!;

    /// <summary>Income date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Category label (Interest, Scrap, Commission…).</summary>
    public string Category { get; set; } = default!;

    /// <summary>Amount received.</summary>
    public decimal Amount { get; set; }

    /// <summary>Tax component, when the receipt carries GST.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>How the money was received.</summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>Free-text description.</summary>
    public string? Description { get; set; }

    /// <summary>Payer reference.</summary>
    public string? ReceivedFrom { get; set; }
}