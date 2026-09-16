using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Catalog;
using BizStock.Domain.Common;
using BizStock.Domain.Inventory;
using Microsoft.EntityFrameworkCore;

using BizStock.Infrastructure.Persistence;
namespace BizStock.Infrastructure.Services;

/// <summary>
/// Stock engine: updates the materialized balance and writes an immutable
/// ledger row in the same save. Enforces the negative-stock policy.
/// </summary>
public class StockService : IStockService
{
    private readonly AppDbContext _db;
    private readonly IBusinessContext _business;

    /// <summary>Creates the stock service.</summary>
    public StockService(AppDbContext db, IBusinessContext business)
    {
        _db = db;
        _business = business;
    }

    /// <inheritdoc />
    public async Task<StockMovement> ApplyMovementAsync(
        Guid productId, StockMovementType type, decimal quantity, decimal unitCost,
        string? reference = null, Guid? documentId = null, string? batchNumber = null,
        string? note = null, DateTime? dateUtc = null, CancellationToken ct = default)
    {
        if (quantity == 0)
        {
            throw new BusinessRuleViolationException("Stock movement quantity cannot be zero.");
        }

        var settings = await _business.GetSettingsAsync(ct);

        var summary = await _db.StockSummaries.FirstOrDefaultAsync(s => s.ProductId == productId, ct);
        if (summary is null)
        {
            summary = new StockSummary
            {
                ProductId = productId,
                Quantity = 0,
                AverageCost = unitCost
            };
            _db.StockSummaries.Add(summary);
        }

        var newQty = summary.Quantity + quantity;
        if (newQty < 0 && !settings.AllowNegativeStock)
        {
            var product = await _db.Products.IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == productId, ct);
            throw new InsufficientStockException(product?.Name ?? productId.ToString(), Math.Abs(quantity), summary.Quantity);
        }

        // Weighted-average cost update on inbound movements.
        if (quantity > 0)
        {
            var totalValue = (summary.Quantity * summary.AverageCost) + (quantity * unitCost);
            summary.AverageCost = totalValue / newQty;
        }
        else
        {
            summary.AverageCost = summary.AverageCost == 0 ? unitCost : summary.AverageCost;
        }

        summary.Quantity = newQty;
        summary.LastMovementAtUtc = dateUtc ?? DateTime.UtcNow;

        var movement = new StockMovement
        {
            ProductId = productId,
            DateUtc = dateUtc ?? DateTime.UtcNow,
            MovementType = type,
            Quantity = quantity,
            BalanceAfter = newQty,
            UnitCost = unitCost,
            BatchNumber = batchNumber,
            Reference = reference,
            DocumentId = documentId,
            Note = note
        };

        _db.StockMovements.Add(movement);
        return movement;
    }

    /// <inheritdoc />
    public async Task<decimal> GetQuantityAsync(Guid productId, CancellationToken ct = default) =>
        await _db.StockSummaries.Where(s => s.ProductId == productId)
            .Select(s => s.Quantity).FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<IReadOnlyList<Guid>> GetLowStockProductIdsAsync(CancellationToken ct = default)
    {
        var settings = await _business.GetSettingsAsync(ct);
        var threshold = settings.LowStockThreshold;

        var query = from s in _db.StockSummaries
                    join p in _db.Products on s.ProductId equals p.Id
                    where s.Quantity <= (p.MinimumStock > 0 ? p.MinimumStock : threshold)
                    select p.Id;

        return await query.ToListAsync(ct);
    }
}
