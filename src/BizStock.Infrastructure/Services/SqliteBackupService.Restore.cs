using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Auditing;
using BizStock.Domain.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using BizStock.Infrastructure.Persistence;
namespace BizStock.Infrastructure.Services;

/// <summary>Restore, listing and scheduled backup logic.</summary>
public partial class SqliteBackupService
{
    /// <inheritdoc />
    public async Task<IBackupService.BackupResult> RestoreBackupAsync(string backupFilePath, CancellationToken ct = default)
    {
        try
        {
            if (!File.Exists(backupFilePath))
            {
                return new IBackupService.BackupResult(false, "Backup file not found.");
            }

            // Safety backup before overwrite; never silently destroy the current database.
            var safety = await CreateBackupAsync(BackupKind.PreRestore,
                $"Safety backup before restoring {Path.GetFileName(backupFilePath)}", ct);
            if (!safety.Success)
            {
                return new IBackupService.BackupResult(false, "Restore aborted: could not create the pre-restore safety backup.");
            }

            var currentPath = ResolveDatabasePath(_db.Database.GetConnectionString());
            if (string.IsNullOrEmpty(currentPath))
            {
                return new IBackupService.BackupResult(false, "Cannot resolve the live database path.");
            }

            // Replace the live database file. The app should restart immediately after.
            File.Copy(backupFilePath, currentPath, overwrite: true);

            return new IBackupService.BackupResult(true, "Database restored. Please restart the application.", currentPath);
        }
        catch (Exception ex)
        {
            return new IBackupService.BackupResult(false, $"Restore failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<BackupLog>> ListBackupsAsync(CancellationToken ct = default) =>
        await _db.BackupLogs.AsNoTracking()
            .OrderByDescending(b => b.CreatedAtUtc)
            .Take(200)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<bool> DeleteBackupAsync(Guid backupLogId, CancellationToken ct = default)
    {
        var log = await _db.BackupLogs.FindAsync([backupLogId], ct);
        if (log is null)
        {
            return false;
        }

        if (File.Exists(log.FullPath))
        {
            File.Delete(log.FullPath);
        }

        _db.BackupLogs.Remove(log);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    /// <inheritdoc />
    public async Task<IBackupService.BackupResult?> RunScheduledBackupIfDueAsync(CancellationToken ct = default)
    {
        var settings = await _db.BusinessSettings.FirstOrDefaultAsync(ct);
        if (settings is null || !settings.BackupEnabled || settings.BackupFrequencyDays <= 0)
        {
            return null;
        }

        if (settings.LastBackupAtUtc is { } last
            && DateTime.UtcNow - last < TimeSpan.FromDays(settings.BackupFrequencyDays))
        {
            return null;
        }

        var result = await CreateBackupAsync(BackupKind.Scheduled, "Automatic scheduled backup", ct);
        if (result.Success)
        {
            settings.LastBackupAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return result;
        }

        return null;
    }

    private async Task<string> GetBackupDirectoryAsync(CancellationToken ct)
    {
        var settings = await _db.BusinessSettings.FirstOrDefaultAsync(ct);
        if (!string.IsNullOrWhiteSpace(settings?.BackupLocation))
        {
            return settings.BackupLocation;
        }

        return Path.Combine(DatabasePaths.GetDefaultDataDirectory(), "backups");
    }

    /// <summary>Extracts the file path from a SQLite connection string.</summary>
    internal static string? ResolveDatabasePath(string? connectionString)
    {
        if (connectionString is null)
        {
            return null;
        }

        var builder = new SqliteConnectionStringBuilder(connectionString);
        return builder.DataSource == ":memory:" ? null : builder.DataSource;
    }
}
