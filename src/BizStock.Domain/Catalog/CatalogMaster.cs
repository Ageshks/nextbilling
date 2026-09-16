using BizStock.Domain.Common;

namespace BizStock.Domain.Catalog;

/// <summary>Product category.</summary>
public class Category : BaseEntity, IActivatable
{
    /// <summary>Unique category name.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Description.</summary>
    public string? Description { get; set; }

    /// <summary>Whether the category is active.</summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>Product brand/manufacturer.</summary>
public class Brand : BaseEntity, IActivatable
{
    /// <summary>Unique brand name.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Description.</summary>
    public string? Description { get; set; }

    /// <summary>Whether the brand is active.</summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>Unit of measure.</summary>
public class Unit : BaseEntity, IActivatable
{
    /// <summary>Unique full unit name, e.g. "Kilogram".</summary>
    public string Name { get; set; } = default!;

    /// <summary>Unique short symbol, e.g. "KG".</summary>
    public string Symbol { get; set; } = default!;

    /// <summary>Whether fractional quantities are allowed.</summary>
    public bool DecimalAllowed { get; set; }

    /// <summary>Whether this is a seeded system unit.</summary>
    public bool IsSystem { get; set; }

    /// <summary>Whether the unit can be selected for new products.</summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>Configurable GST rate (e.g. 0, 5, 12, 18, 28). Never hard-coded in logic.</summary>
public class TaxRate : BaseEntity, IActivatable
{
    /// <summary>Display name, e.g. "GST 18%".</summary>
    public string Name { get; set; } = default!;

    /// <summary>Rate percent.</summary>
    public decimal RatePercent { get; set; }

    /// <summary>Whether this is the suggested default rate for new products.</summary>
    public bool IsDefault { get; set; }

    /// <summary>Whether the rate can be selected.</summary>
    public bool IsActive { get; set; } = true;
}
