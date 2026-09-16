using BizStock.Application.Common.Exceptions;
using BizStock.Application.Common.Interfaces;

namespace BizStock.Infrastructure.Services;

/// <summary>
/// Process-wide session holder registered as a singleton. Authentication state must
/// outlive the per-use-case DI scopes, so it cannot live in a scoped service.
/// </summary>
public sealed class SessionService : ISessionService
{
    private readonly object _gate = new();

    /// <inheritdoc />
    public SessionInfo? Current { get; private set; }

    /// <inheritdoc />
    public event EventHandler<SessionInfo?>? Changed;

    /// <inheritdoc />
    public void Set(SessionInfo? session)
    {
        lock (_gate)
        {
            Current = session;
        }

        Changed?.Invoke(this, session);
    }

    /// <inheritdoc />
    public bool IsPermitted(string permissionKey) => Current?.Has(permissionKey) ?? false;

    /// <inheritdoc />
    public SessionInfo RequireSession() =>
        Current ?? throw new AuthorizationException("session:authenticated");
}
