using BizStock.Domain.Common;
using BizStock.Domain.Finance;

namespace BizStock.Application.Common.Interfaces;

/// <summary>
/// Double-entry posting engine. All financial transactions route through this
/// service so the books always balance. Each method validates balance before saving.
/// </summary>
public interface IAccountingService
{
    /// <summary>Posts a balanced journal to the ledger and returns its id.</summary>
    Task<Guid> PostAsync(JournalEntry entry, CancellationToken ct = default);

    /// <summary>Posts a simple two-sided journal (one debit, one credit).</summary>
    Task<Guid> PostSimpleAsync(
        DateTime dateUtc,
        VoucherType type,
        string reference,
        Guid debitAccountId,
        Guid creditAccountId,
        decimal amount,
        string? narration = null,
        Guid? documentId = null,
        CancellationToken ct = default);

    /// <summary>Resolves the system account id for a well-known code, creating it if missing.</summary>
    Task<Guid> GetSystemAccountIdAsync(string code, CancellationToken ct = default);
}
