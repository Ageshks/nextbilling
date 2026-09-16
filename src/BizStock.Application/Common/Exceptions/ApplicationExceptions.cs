namespace BizStock.Application.Common.Exceptions;

/// <summary>Raised when input fails a use-case level validation rule.</summary>
public class AppValidationException : Exception
{
    /// <summary>Creates the exception with a user-facing message.</summary>
    public AppValidationException(string message) : base(message)
    {
    }

    /// <summary>Creates the exception from a set of field errors.</summary>
    public AppValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base(string.Join(Environment.NewLine, errors.SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}"))))
    {
        Errors = errors;
    }

    /// <summary>Field-level errors, when available.</summary>
    public IReadOnlyDictionary<string, string[]>? Errors { get; }
}

/// <summary>Raised when a requested record does not exist or has been deactivated.</summary>
public class NotFoundException : Exception
{
    /// <summary>Creates the exception naming the entity type.</summary>
    public NotFoundException(string entityName)
        : base($"{entityName} was not found.")
    {
        EntityName = entityName;
    }

    /// <summary>Name of the missing entity type.</summary>
    public string EntityName { get; }
}

/// <summary>Raised when the signed-in user lacks the permission required for an action.</summary>
public class AuthorizationException : Exception
{
    /// <summary>Creates the exception naming the missing permission.</summary>
    public AuthorizationException(string permissionKey)
        : base($"You do not have permission to perform this action ({permissionKey}).")
    {
        PermissionKey = permissionKey;
    }

    /// <summary>The permission key that was required.</summary>
    public string PermissionKey { get; }
}