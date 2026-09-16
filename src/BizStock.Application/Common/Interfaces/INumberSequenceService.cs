using BizStock.Domain.Common;

namespace BizStock.Application.Common.Interfaces;

/// <summary>
/// Transactional document number generator. Guarantees gap-free, duplicate-free
/// numbers per document type by serializing allocation inside the save transaction.
/// </summary>
public interface INumberSequenceService
{
    /// <summary>Allocates the next number for a document type, e.g. INV-2026-000042.</summary>
    Task<string> GetNextNumberAsync(DocumentType type, CancellationToken ct = default);

    /// <summary>Peeks the next number without consuming it (UI preview).</summary>
    Task<string> PeekNextNumberAsync(DocumentType type, CancellationToken ct = default);
}
