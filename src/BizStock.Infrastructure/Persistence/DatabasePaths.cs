namespace BizStock.Infrastructure.Persistence;

/// <summary>Resolves the default data directory and connection string for the application database.</summary>
public static class DatabasePaths
{
    public static string GetDefaultDataDirectory()
    {
        var configured = Environment.GetEnvironmentVariable("BIZSTOCK_DATA_DIR");
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured;
        }

        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BizStock");
        }

        if (OperatingSystem.IsMacOS())
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Library", "Application Support", "BizStock");
        }

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".local", "share", "BizStock");
    }

    public static string GetDataDirectory(string? dataDirectory = null)
    {
        return dataDirectory ?? GetDefaultDataDirectory();
    }

    public static string GetDatabaseDirectory(string? dataDirectory = null)
    {
        return Path.Combine(GetDataDirectory(dataDirectory), "Data");
    }

    public static string GetBackupsDirectory(string? dataDirectory = null)
    {
        return Path.Combine(GetDataDirectory(dataDirectory), "Backups");
    }

    public static string GetLogsDirectory(string? dataDirectory = null)
    {
        return Path.Combine(GetDataDirectory(dataDirectory), "Logs");
    }

    public static string GetDocumentsDirectory(string? dataDirectory = null)
    {
        return Path.Combine(GetDataDirectory(dataDirectory), "Documents");
    }

    public static string GetExportsDirectory(string? dataDirectory = null)
    {
        return Path.Combine(GetDataDirectory(dataDirectory), "Exports");
    }

    public static void EnsureDataDirectory(string? dataDirectory = null)
    {
        var root = GetDataDirectory(dataDirectory);
        Directory.CreateDirectory(root);
        Directory.CreateDirectory(GetDatabaseDirectory(root));
        Directory.CreateDirectory(GetBackupsDirectory(root));
        Directory.CreateDirectory(GetLogsDirectory(root));
        Directory.CreateDirectory(GetDocumentsDirectory(root));
        Directory.CreateDirectory(GetExportsDirectory(root));
    }

    public static string GetDatabasePath(string? dataDirectory = null)
    {
        var dir = GetDatabaseDirectory(dataDirectory);
        return Path.Combine(dir, "bizstock.db");
    }

    public static string GetConnectionString(string? dataDirectory = null)
    {
        var path = GetDatabasePath(dataDirectory);
        return new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder
        {
            DataSource = path,
            Mode = Microsoft.Data.Sqlite.SqliteOpenMode.ReadWriteCreate,
            ForeignKeys = true
        }.ToString();
    }
}
