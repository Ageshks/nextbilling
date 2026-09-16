using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Auditing;
using BizStock.Domain.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using BizStock.Infrastructure.Persistence;
namespace BizStock.Infrastructure.Services;

/// <summary>SQLite backup/restore using the engine's online backup API.</summary>
public partial class SqliteBackupService : IBackupService
{
    private readonly AppDbContext _db;
    private readonly IAuthService _auth;

    /// <summary>Creates the backup service.</summary>
    public SqliteBackupService(AppDbContext db, IAuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    /// <inheritdoc />
    public async Task<IBackupService.BackupResult> CreateBackupAsync(
        BackupKind kind = BackupKind.Manual, string? note = null, CancellationToken ct = default)
    {
        try
        {
            var location = await GetBackupDirectoryAsync(ct);
            Directory.CreateDirectory(location);

            var fileName = $"bizstock-{kind.ToString().ToLowerInvariant()}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.db";
            var targetPath = Path.Combine(location, fileName);
            var connectionString = _db.Database.GetConnectionString();

            await using var source = new SqliteConnection(connectionString);
            await source.OpenAsync(ct);
            await using var destination = new SqliteConnection($"Data Source={targetPath}");
            await destination.OpenAsync(ct);
            source.BackupDatabase(destination);

            var log = new BackupLog
            {
                FileName = fileName,
                FullPath = targetPath,
                SizeBytes = new FileInfo(targetPath).Length,
                Kind = kind,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUsername = _auth.CurrentSession?.Username,
                Note = note
            };
            _db.BackupLogs.Add(log);
            await _db.SaveChangesAsync(ct);

            return new IBackupService.BackupResult(true, $"Backup created: {fileName}", targetPath);
        }
        catch (Exception ex)
        {
            return new IBackupService.BackupResult(false, $"Backup failed: {ex.Message}");
        }
    }
}
