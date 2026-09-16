using BizStock.Application.Catalog;
using BizStock.Application.Common.Models;
using BizStock.Domain.Catalog;

namespace BizStock.Application.Common.Interfaces;

/// <summary>
/// Product/category/brand/unit catalogue operations. Owns product-code and
/// barcode uniqueness rules and creates the stock summary row for new products.
/// </summary>
public interface ICatalogService
{
    // ---- Products ----
    /// <summary>Searches products with server-side paging, filtering and sorting.</summary>
    Task<PagedResult<ProductListItem>> SearchProductsAsync(ProductQuery query, CancellationToken ct = default);

    /// <summary>Loads a product for editing.</summary>
    Task<ProductDetailDto?> GetProductAsync(Guid id, CancellationToken ct = default);

    /// <summary>Fast POS/global lookup across name, code, SKU and barcode.</summary>
    Task<IReadOnlyList<ProductLookupDto>> LookupProductsAsync(string term, int take = 20, CancellationToken ct = default);

    /// <summary>Resolves a scanned barcode (or SKU/product code) to a single product.</summary>
    Task<ProductLookupDto?> FindByBarcodeAsync(string code, CancellationToken ct = default);

    /// <summary>Creates or updates a product; validates unique code/SKU/barcode.</summary>
    Task<Guid> SaveProductAsync(ProductDetailDto product, CancellationToken ct = default);

    /// <summary>Activates or deactivates a product (soft, history-preserving).</summary>
    Task SetProductActiveAsync(Guid id, bool isActive, CancellationToken ct = default);

    /// <summary>Deletes a product softly; blocked when stock or history exists.</summary>
    Task DeleteProductAsync(Guid id, CancellationToken ct = default);

    /// <summary>Next generated product code (preview for the editor).</summary>
    Task<string> PeekNextProductCodeAsync(CancellationToken ct = default);

    // ---- Categories ----
    /// <summary>Categories with product counts.</summary>
    Task<IReadOnlyList<CategoryListItem>> GetCategoriesAsync(CancellationToken ct = default);

    /// <summary>Creates or updates a category.</summary>
    Task<Guid> SaveCategoryAsync(Guid? id, string name, string? description, bool isActive, CancellationToken ct = default);

    /// <summary>Deactivates a category.</summary>
    Task SetCategoryActiveAsync(Guid id, bool isActive, CancellationToken ct = default);

    // ---- Brands ----
    /// <summary>Brands with product counts.</summary>
    Task<IReadOnlyList<BrandListItem>> GetBrandsAsync(CancellationToken ct = default);

    /// <summary>Creates or updates a brand.</summary>
    Task<Guid> SaveBrandAsync(Guid? id, string name, string? description, bool isActive, CancellationToken ct = default);

    /// <summary>Deactivates a brand.</summary>
    Task SetBrandActiveAsync(Guid id, bool isActive, CancellationToken ct = default);

    // ---- Units ----
    /// <summary>Units of measure with product counts.</summary>
    Task<IReadOnlyList<UnitListItem>> GetUnitsAsync(CancellationToken ct = default);

    /// <summary>Creates or updates a custom unit.</summary>
    Task<Guid> SaveUnitAsync(Guid? id, string name, string symbol, bool decimalAllowed, bool isActive, CancellationToken ct = default);

    /// <summary>Deactivates a unit.</summary>
    Task SetUnitActiveAsync(Guid id, bool isActive, CancellationToken ct = default);

    // ---- Tax rates ----
    /// <summary>Configurable GST rates.</summary>
    Task<IReadOnlyList<TaxRate>> GetTaxRatesAsync(bool includeInactive = false, CancellationToken ct = default);

    /// <summary>Adds or updates a GST rate.</summary>
    Task<Guid> SaveTaxRateAsync(Guid? id, string name, decimal ratePercent, bool isDefault, bool isActive, CancellationToken ct = default);
}