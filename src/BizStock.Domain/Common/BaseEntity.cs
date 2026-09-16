namespace BizStock.Domain.Common;

/// <summary>
/// Base type for all persisted entities. Timestamps are stored in UTC.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>Last update timestamp (UTC).</summary>
    public DateTime UpdatedAtUtc { get; set; }
}

/// <summary>Implemented by entities that use soft delete instead of physical delete.</summary>
public interface ISoftDeletable
{
    /// <summary>Deletion timestamp (UTC), or null while the record is alive.</summary>
    DateTime? DeletedAtUtc { get; set; }
}

/// <summary>Implemented by master data entities that can be deactivated without deleting.</summary>
public interface IActivatable
{
    /// <summary>Whether the record is active and usable in new transactions.</summary>
    bool IsActive { get; set; }
}

/// <summary>Implemented by entities that belong to a business, preparing future multi-tenant support.</summary>
public interface IBusinessScoped
{
    /// <summary>Owning business identifier. Assigned automatically on save when null.</summary>
    Guid? BusinessId { get; set; }
}
