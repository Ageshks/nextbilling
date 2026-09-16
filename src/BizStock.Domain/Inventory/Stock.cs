using BizStock.Domain.Common;
using BizStock.Domain.Catalog;

namespace BizStock.Domain.Inventory;

/// <summary>Materialized current stock balance per product.</summary>
public class StockSummary : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Product identifier.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Current quantity on hand. Never negative unless configured.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Reserved quantity (Phase 10+).</summary>
    public decimal ReservedQuantity { get; set; }

    /// <summary>Weighted-average cost of the on-hand stock.</summary>
    public decimal AverageCost { get; set; }

    /// <summary>Last movement timestamp (UTC).</summary>
    public DateTime? LastMovementAtUtc { get; set; }

    /// <summary>Product navigation.</summary>
    public Product Product { get; set; } = default!;
}

/// <summary>Immutable stock ledger line. Every quantity change writes one row inside the same transaction.</summary>
public class StockMovement : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Movement timestamp (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Product identifier.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Movement category.</summary>
    public StockMovementType MovementType { get; set; }

    /// <summary>Positive quantity in / negative out.</summary>
    public decimal Quantity { get; set; }

    /// <summary>Quantity on hand after this movement.</summary>
    public decimal BalanceAfter { get; set; }

    /// <summary>Unit cost of this movement for valuation.</summary>
    public decimal UnitCost { get; set; }

    /// <summary>Optional batch number.</summary>
    public string? BatchNumber { get; set; }

    /// <summary>Reference document number.</summary>
    public string? Reference { get; set; }

    /// <summary>Source document id.</summary>
    public Guid? DocumentId { get; set; }

    /// <summary>Optional note.</summary>
    public string? Note { get; set; }

    /// <summary>Product navigation.</summary>
    public Product Product { get; set; } = default!;
}

/// <summary>Stock adjustment document header (re-count, damage, expiry…).</summary>
public class StockAdjustment : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique adjustment number, e.g. ADJ-000001.</summary>
    public string AdjustmentNumber { get; set; } = default!;

    /// <summary>Adjustment date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Adjustment reason.</summary>
    public AdjustmentReason Reason { get; set; }

    /// <summary>Posted / cancelled.</summary>
    public AdjustmentStatus Status { get; set; }

    /// <summary>Note.</summary>
    public string? Note { get; set; }

    /// <summary>Lines.</summary>
    public List<StockAdjustmentItem> Items { get; set; } = [];
}

/// <summary>Stock adjustment line.</summary>
public class StockAdjustmentItem : BaseEntity
{
    /// <summary>Parent adjustment id.</summary>
    public Guid StockAdjustmentId { get; set; }

    /// <summary>Product identifier.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Signed quantity change (+/-).</summary>
    public decimal Quantity { get; set; }

    /// <summary>Unit cost used for valuation.</summary>
    public decimal UnitCost { get; set; }

    /// <summary>Optional note.</summary>
    public string? Note { get; set; }
}
