using BizStock.Domain.Auditing;
using BizStock.Domain.Common;

namespace BizStock.Application.Common.Interfaces;

/// <summary>Database backup and restore operations.</summary>
public interface IBackupService
{
    /// <summary>Result of a backup or restore.</summary>
    public record BackupResult(bool Success, string Message, string? FilePath = null);

    /// <summary>Creates a database backup file.</summary>
    Task<BackupResult> CreateBackupAsync(BackupKind kind = BackupKind.Manual, string? note = null, CancellationToken ct = default);

    /// <summary>Restores a database from a backup file (pre-restore safety backup taken automatically).</summary>
    Task<BackupResult> RestoreBackupAsync(string backupFilePath, CancellationToken ct = default);

    /// <summary>Lists backup files from the configured backup location.</summary>
    Task<IReadOnlyList<BackupLog>> ListBackupsAsync(CancellationToken ct = default);

    /// <summary>Deletes a backup file from disk.</summary>
    Task<bool> DeleteBackupAsync(Guid backupLogId, CancellationToken ct = default);

    /// <summary>Runs the scheduled automatic backup if due; returns the backup if one was taken.</summary>
    Task<BackupResult?> RunScheduledBackupIfDueAsync(CancellationToken ct = default);
}
