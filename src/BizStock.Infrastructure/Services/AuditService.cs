using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Auditing;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using BizStock.Infrastructure.Persistence;
namespace BizStock.Infrastructure.Services;

/// <summary>Audit trail writer; failures are swallowed so audit cannot break business flows.</summary>
public class AuditService : IAuditService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        MaxDepth = 4
    };

    private readonly AppDbContext _db;

    /// <summary>Creates the audit service.</summary>
    public AuditService(AppDbContext db, IAuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    private readonly IAuthService _auth;

    /// <inheritdoc />
    public async Task RecordAsync(string action, string module, string? entity = null, string? entityId = null,
        object? oldValues = null, object? newValues = null, CancellationToken ct = default)
    {
        try
        {
            _db.AuditLogs.Add(new AuditLog
            {
                UserId = _auth.CurrentSession?.UserId,
                Username = _auth.CurrentSession?.Username,
                Action = action,
                Module = module,
                Entity = entity,
                EntityId = entityId,
                OldValues = Serialize(oldValues),
                NewValues = Serialize(newValues),
                TimestampUtc = DateTime.UtcNow
            });
            await _db.SaveChangesAsync(ct);
        }
        catch
        {
            // Audit must never break the business flow; log-lessly continue.
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditLog>> QueryAsync(string? module = null, string? action = null,
        DateTime? fromUtc = null, DateTime? toUtc = null, int take = 500, CancellationToken ct = default)
    {
        var query = _db.AuditLogs.AsNoTracking().AsQueryable();
        if (module is not null)
        {
            query = query.Where(a => a.Module == module);
        }
        if (action is not null)
        {
            query = query.Where(a => a.Action == action);
        }
        if (fromUtc is not null)
        {
            query = query.Where(a => a.TimestampUtc >= fromUtc);
        }
        if (toUtc is not null)
        {
            query = query.Where(a => a.TimestampUtc <= toUtc);
        }

        return await query.OrderByDescending(a => a.TimestampUtc).Take(take).ToListAsync(ct);
    }

    private static string? Serialize(object? value) =>
        value is null ? null : JsonSerializer.Serialize(value, JsonOptions);
}
