using BizStock.Domain.Common;

namespace BizStock.Domain.Settings;

/// <summary>Simple key/value application setting.</summary>
public class SettingEntry : BaseEntity
{
    /// <summary>Unique setting key.</summary>
    public string Key { get; set; } = default!;

    /// <summary>Setting value (string encoded).</summary>
    public string? Value { get; set; }

    /// <summary>Human-readable description.</summary>
    public string? Description { get; set; }

    /// <summary>Well-known setting keys.</summary>
    public static class Keys
    {
        /// <summary>Username remembered on the login screen.</summary>
        public const string RememberedUsername = "Login.RememberedUsername";

        /// <summary>Whether the last run created a backup automatically.</summary>
        public const string LastAutoBackupAt = "Backup.LastAutoBackupAt";

        /// <summary>Application schema version for upgrade checks.</summary>
        public const string SchemaVersion = "App.SchemaVersion";
    }
}
