using BizStock.Domain.Common;

namespace BizStock.Domain.Identity;

/// <summary>Application user with credential material and account state.</summary>
public class User : BaseEntity
{
    /// <summary>Unique login name.</summary>
    public string Username { get; set; } = default!;

    /// <summary>Display name shown in the UI.</summary>
    public string DisplayName { get; set; } = default!;

    /// <summary>Optional email address.</summary>
    public string? Email { get; set; }

    /// <summary>Versioned password hash. Never store plain-text passwords.</summary>
    public string PasswordHash { get; set; } = default!;

    /// <summary>Whether the account is active and may log in.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>When true the user must change the password at next login.</summary>
    public bool MustChangePassword { get; set; }

    /// <summary>Last successful login (UTC).</summary>
    public DateTime? LastLoginAtUtc { get; set; }

    /// <summary>Consecutive failed login attempts.</summary>
    public int FailedLoginCount { get; set; }

    /// <summary>Account lock expiry after repeated failures (UTC).</summary>
    public DateTime? LockedUntilUtc { get; set; }

    /// <summary>Optional notes.</summary>
    public string? Notes { get; set; }

    /// <summary>Role memberships.</summary>
    public List<UserRole> UserRoles { get; set; } = [];
}

/// <summary>Join entity: user in role.</summary>
public class UserRole
{
    /// <summary>User identifier.</summary>
    public Guid UserId { get; set; }

    /// <summary>Role identifier.</summary>
    public Guid RoleId { get; set; }

    /// <summary>Role navigation.</summary>
    public Role Role { get; set; } = default!;
}
