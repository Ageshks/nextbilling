using BizStock.Domain.Common;

namespace BizStock.Domain.Identity;

/// <summary>Named security role grouping a set of permissions.</summary>
public class Role : BaseEntity
{
    /// <summary>Unique role name.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Description.</summary>
    public string? Description { get; set; }

    /// <summary>System roles cannot be deleted.</summary>
    public bool IsSystemRole { get; set; }

    /// <summary>Permissions granted to this role.</summary>
    public List<RolePermission> RolePermissions { get; set; } = [];

    /// <summary>User memberships.</summary>
    public List<UserRole> UserRoles { get; set; } = [];
}

/// <summary>Join entity: role has permission.</summary>
public class RolePermission
{
    /// <summary>Role identifier.</summary>
    public Guid RoleId { get; set; }

    /// <summary>Permission identifier.</summary>
    public Guid PermissionId { get; set; }

    /// <summary>Permission navigation.</summary>
    public Permission Permission { get; set; } = default!;
}
