namespace BizStock.Application.Common.Interfaces;

/// <summary>
/// Holds the signed-in session for the life of the desktop process.
/// Implemented as a singleton so authentication state survives the short-lived
/// dependency-injection scopes that each use case runs in.
/// </summary>
public interface ISessionService
{
    /// <summary>Currently signed-in session, or null when nobody is signed in.</summary>
    SessionInfo? Current { get; }

    /// <summary>Raised whenever the session is established or cleared.</summary>
    event EventHandler<SessionInfo?>? Changed;

    /// <summary>Replaces the current session (null = signed out).</summary>
    void Set(SessionInfo? session);

    /// <summary>Whether the signed-in user holds the given permission key.</summary>
    bool IsPermitted(string permissionKey);

    /// <summary>Returns the current session or throws when nobody is signed in.</summary>
    SessionInfo RequireSession();
}