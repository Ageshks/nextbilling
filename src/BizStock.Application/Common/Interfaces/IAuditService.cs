using BizStock.Domain.Auditing;

namespace BizStock.Application.Common.Interfaces;

/// <summary>Audit trail writer. Never throws to the caller; audit failures are logged, not fatal.</summary>
public interface IAuditService
{
    /// <summary>Records an audit entry for the current session user.</summary>
    Task RecordAsync(string action, string module, string? entity = null, string? entityId = null,
        object? oldValues = null, object? newValues = null, CancellationToken ct = default);

    /// <summary>Queries the audit trail.</summary>
    Task<IReadOnlyList<AuditLog>> QueryAsync(string? module = null, string? action = null,
        DateTime? fromUtc = null, DateTime? toUtc = null, int take = 500, CancellationToken ct = default);
}
