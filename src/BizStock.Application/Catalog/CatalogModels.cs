using BizStock.Application.Common.Models;

namespace BizStock.Application.Catalog;

/// <summary>Product list projection (never materialises full entities for grids).</summary>
public sealed record ProductListItem(
    Guid Id,
    string ProductCode,
    string Name,
    string? SKU,
    string? Barcode,
    string? CategoryName,
    string? BrandName,
    string? UnitSymbol,
    decimal PurchasePrice,
    decimal SellingPrice,
    decimal MRP,
    decimal GSTRate,
    decimal StockQuantity,
    decimal MinimumStock,
    bool IsActive);

/// <summary>Full product detail projection.</summary>
public sealed record ProductDetailDto(
    Guid Id,
    string ProductCode,
    string Name,
    string? SKU,
    string? Barcode,
    Guid? CategoryId,
    Guid? BrandId,
    Guid? UnitId,
    Guid? SupplierId,
    decimal PurchasePrice,
    decimal SellingPrice,
    decimal WholesalePrice,
    decimal MRP,
    decimal GSTRate,
    string? HSNCode,
    decimal MinimumStock,
    decimal? MaximumStock,
    string? BatchNumber,
    DateTime? ExpiryDateUtc,
    string? ImagePath,
    string? Description,
    bool IsActive,
    decimal StockQuantity);

/// <summary>Product search criteria.</summary>
public sealed class ProductQuery : PageRequest
{
    /// <summary>Filter by category.</summary>
    public Guid? CategoryId { get; set; }

    /// <summary>Filter by brand.</summary>
    public Guid? BrandId { get; set; }

    /// <summary>Filter by active state.</summary>
    public bool? IsActive { get; set; }

    /// <summary>Only products at or below their minimum stock.</summary>
    public bool LowStockOnly { get; set; }

    /// <summary>Only products with zero or negative on-hand quantity.</summary>
    public bool OutOfStockOnly { get; set; }

    /// <summary>Only products that carry an expiry date (batch/expiry tracking).</summary>
    public bool WithExpiryOnly { get; set; }
}

/// <summary>Lightweight product lookup row used by POS search and dropdowns.</summary>
public sealed record ProductLookupDto(
    Guid Id,
    string ProductCode,
    string Name,
    string? Barcode,
    decimal SellingPrice,
    decimal PurchasePrice,
    decimal GSTRate,
    decimal StockQuantity,
    string? UnitSymbol);

/// <summary>Category dropdown row with product counts.</summary>
public sealed record CategoryListItem(Guid Id, string Name, string? Description, bool IsActive, int ProductCount);

/// <summary>Brand dropdown row with product counts.</summary>
public sealed record BrandListItem(Guid Id, string Name, string? Description, bool IsActive, int ProductCount);

/// <summary>Unit list row.</summary>
public sealed record UnitListItem(Guid Id, string Name, string Symbol, bool DecimalAllowed, bool IsSystem, bool IsActive, int ProductCount);