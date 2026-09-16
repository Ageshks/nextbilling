using BizStock.Domain.Common;

namespace BizStock.Domain.Billing;

/// <summary>Counter machine configuration.</summary>
public class BillingMachine : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Machine name.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Whether this is the default machine.</summary>
    public bool IsDefault { get; set; }

    /// <summary>Note.</summary>
    public string? Note { get; set; }
}
