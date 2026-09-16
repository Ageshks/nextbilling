using BizStock.Domain.Common;

namespace BizStock.Domain.Auditing;

/// <summary>Immutable audit trail record for sensitive operations.</summary>
public class AuditLog
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Acting user identifier, if authenticated.</summary>
    public Guid? UserId { get; set; }

    /// <summary>Acting username snapshot.</summary>
    public string? Username { get; set; }

    /// <summary>Action performed, e.g. "SaleCreated".</summary>
    public string Action { get; set; } = default!;

    /// <summary>Module the action belongs to.</summary>
    public string? Module { get; set; }

    /// <summary>Entity type affected, e.g. "Sale".</summary>
    public string? Entity { get; set; }

    /// <summary>Entity identifier as string.</summary>
    public string? EntityId { get; set; }

    /// <summary>Serialized previous state (JSON). Never contains credentials.</summary>
    public string? OldValues { get; set; }

    /// <summary>Serialized new state (JSON). Never contains credentials.</summary>
    public string? NewValues { get; set; }

    /// <summary>When the action happened (UTC).</summary>
    public DateTime TimestampUtc { get; set; }
}

/// <summary>History of database backups and restores.</summary>
public class BackupLog
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Backup file name.</summary>
    public string FileName { get; set; } = default!;

    /// <summary>Full path of the backup file.</summary>
    public string FullPath { get; set; } = default!;

    /// <summary>File size in bytes.</summary>
    public long SizeBytes { get; set; }

    /// <summary>Backup kind.</summary>
    public BackupKind Kind { get; set; }

    /// <summary>When the backup was taken (UTC).</summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>Username that triggered the backup.</summary>
    public string? CreatedByUsername { get; set; }

    /// <summary>Optional note.</summary>
    public string? Note { get; set; }
}
