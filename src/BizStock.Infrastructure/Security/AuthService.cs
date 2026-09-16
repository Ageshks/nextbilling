using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Identity;
using Microsoft.EntityFrameworkCore;

using BizStock.Infrastructure.Persistence;
namespace BizStock.Infrastructure.Security;

/// <summary>Session-scoped authentication service backed by the application database.</summary>
public class AuthService : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

    private readonly AppDbContext _db;
    private readonly PasswordHasher _hasher;
    private readonly object _gate = new();

    /// <summary>Creates the auth service.</summary>
    public AuthService(AppDbContext db, PasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    /// <inheritdoc />
    public SessionInfo? CurrentSession { get; private set; }

    /// <inheritdoc />
    public event EventHandler<SessionInfo?>? SessionChanged;

    /// <inheritdoc />
    public async Task<LoginResultKind> LoginAsync(string username, string password)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user is null || !user.IsActive || string.IsNullOrEmpty(user.PasswordHash))
        {
            return LoginResultKind.InvalidCredentials;
        }

        if (user.LockedUntilUtc is { } until && until > DateTime.UtcNow)
        {
            return LoginResultKind.LockedOut;
        }

        if (!_hasher.Verify(password, user.PasswordHash))
        {
            user.FailedLoginCount++;
            if (user.FailedLoginCount >= MaxFailedAttempts)
            {
                user.LockedUntilUtc = DateTime.UtcNow.Add(LockDuration);
                user.FailedLoginCount = 0;
            }
            await _db.SaveChangesAsync();
            return LoginResultKind.InvalidCredentials;
        }

        user.FailedLoginCount = 0;
        user.LockedUntilUtc = null;
        user.LastLoginAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Key)
            .Distinct()
            .ToList();

        var session = new SessionInfo(user.Id, user.Username, user.DisplayName, permissions);
        CurrentSession = session;
        SessionChanged?.Invoke(this, session);

        return user.MustChangePassword ? LoginResultKind.MustChangePassword : LoginResultKind.Success;
    }

    /// <inheritdoc />
    public void Logout()
    {
        CurrentSession = null;
        SessionChanged?.Invoke(this, null);
    }

    /// <inheritdoc />
    public async Task<bool> ChangeOwnPasswordAsync(string currentPassword, string newPassword)
    {
        if (CurrentSession is null)
        {
            return false;
        }

        var user = await _db.Users.FindAsync([CurrentSession.UserId]);
        if (user is null || !_hasher.Verify(currentPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = _hasher.Hash(newPassword);
        user.MustChangePassword = false;
        await _db.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task ResetPasswordAsync(Guid userId, string newPassword)
    {
        var user = await _db.Users.FindAsync([userId])
            ?? throw new InvalidOperationException("User not found.");
        user.PasswordHash = _hasher.Hash(newPassword);
        user.MustChangePassword = false;
        user.FailedLoginCount = 0;
        user.LockedUntilUtc = null;
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<bool> HasAnyUserAsync() =>
        await _db.Users.AnyAsync(u => u.IsActive);

    /// <inheritdoc />
    public async Task<SessionInfo> CreateInitialAdminAsync(string username, string password, string displayName)
    {
        var adminRole = await _db.Roles.Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Name == "Administrator")
            ?? throw new InvalidOperationException("Administrator role is missing; run database seed.");

        var user = new User
        {
            Username = username.Trim(),
            DisplayName = displayName.Trim(),
            PasswordHash = _hasher.Hash(password),
            MustChangePassword = false,
            IsActive = true
        };
        user.UserRoles.Add(new UserRole { RoleId = adminRole.Id });
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var permissions = adminRole.RolePermissions.Select(rp => rp.Permission.Key).ToList();
        var session = new SessionInfo(user.Id, user.Username, user.DisplayName, permissions);
        CurrentSession = session;
        SessionChanged?.Invoke(this, session);
        return session;
    }

    /// <inheritdoc />
    public bool IsPermitted(string permissionKey)
    {
        lock (_gate)
        {
            return CurrentSession?.Has(permissionKey) == true;
        }
    }
}
