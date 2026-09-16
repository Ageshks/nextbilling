using BizStock.Domain.Identity;

namespace BizStock.Application.Common.Interfaces;

/// <summary>Result of an authentication attempt.</summary>
public enum LoginResultKind
{
    /// <summary>Successful login.</summary>
    Success,

    /// <summary>Unknown or inactive user / wrong password.</summary>
    InvalidCredentials,

    /// <summary>Account locked due to failed attempts.</summary>
    LockedOut,

    /// <summary>Password change required before use.</summary>
    MustChangePassword
}

/// <summary>Details of the signed-in session.</summary>
public record SessionInfo(Guid UserId, string Username, string DisplayName, IReadOnlyList<string> Permissions)
{
    /// <summary>Whether the user holds a given permission key.</summary>
    public bool Has(string permissionKey) => Permissions.Contains(permissionKey);

    /// <summary>Whether the user holds any of the given permission keys.</summary>
    public bool HasAny(params string[] keys) => keys.Any(Has);
}

/// <summary>Authentication service abstraction.</summary>
public interface IAuthService
{
    /// <summary>Currently signed-in session, or null.</summary>
    SessionInfo? CurrentSession { get; }

    /// <summary>Event raised after a successful login or any logout.</summary>
    event EventHandler<SessionInfo?>? SessionChanged;

    /// <summary>Attempts to sign in a user.</summary>
    Task<LoginResultKind> LoginAsync(string username, string password);

    /// <summary>Signs out the current user.</summary>
    void Logout();

    /// <summary>Changes the current user's password (verifying the old one first).</summary>
    Task<bool> ChangeOwnPasswordAsync(string currentPassword, string newPassword);

    /// <summary>Admin password reset; also clears lock and must-change flags.</summary>
    Task ResetPasswordAsync(Guid userId, string newPassword);

    /// <summary>Whether the application has any usable admin account (first-run check).</summary>
    Task<bool> HasAnyUserAsync();

    /// <summary>Creates the first admin account during initial setup.</summary>
    Task<SessionInfo> CreateInitialAdminAsync(string username, string password, string displayName);

    /// <summary>Whether the current session is allowed the given permission.</summary>
    bool IsPermitted(string permissionKey);
}
