using BizStock.Domain.Common;
using BizStock.Domain.Inventory;

namespace BizStock.Application.Common.Interfaces;

/// <summary>
/// Stock movement engine. Applies quantity changes to StockSummary while writing
/// immutable StockMovement rows, enforcing the negative-stock policy.
/// </summary>
public interface IStockService
{
    /// <summary>
    /// Applies a stock movement. Positive quantities move in; negative move out.
    /// Throws <see cref="InsufficientStockException"/> when out-movement would go
    /// below zero and negative stock is disabled.
    /// </summary>
    Task<StockMovement> ApplyMovementAsync(
        Guid productId,
        StockMovementType type,
        decimal quantity,
        decimal unitCost,
        string? reference = null,
        Guid? documentId = null,
        string? batchNumber = null,
        string? note = null,
        DateTime? dateUtc = null,
        CancellationToken ct = default);

    /// <summary>Current quantity on hand for a product.</summary>
    Task<decimal> GetQuantityAsync(Guid productId, CancellationToken ct = default);

    /// <summary>Product ids whose quantity is at or below their minimum stock.</summary>
    Task<IReadOnlyList<Guid>> GetLowStockProductIdsAsync(CancellationToken ct = default);
}
