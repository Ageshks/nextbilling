using BizStock.Application.Catalog;
using BizStock.Application.Common.Exceptions;
using BizStock.Application.Common.Interfaces;
using BizStock.Application.Common.Models;
using BizStock.Domain.Catalog;
using BizStock.Domain.Common;
using BizStock.Domain.Inventory;
using BizStock.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

using BizStock.Infrastructure.Persistence;
namespace BizStock.Infrastructure.Services;

/// <summary>
/// Product / category / brand / unit / tax-rate catalogue operations.
/// Owns product-code generation, uniqueness validation and the stock summary row
/// created alongside every new product.
/// </summary>
public sealed class CatalogService : ICatalogService
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;
    private readonly INumberSequenceService _numbers;

    /// <summary>Creates the catalogue service.</summary>
    public CatalogService(AppDbContext db, IAuditService audit, INumberSequenceService numbers)
    {
        _db = db;
        _audit = audit;
        _numbers = numbers;
    }

    // ---------------------------------------------------------------- products

    /// <inheritdoc />
    public async Task<PagedResult<ProductListItem>> SearchProductsAsync(ProductQuery query, CancellationToken ct = default)
    {
        var q = _db.Products.AsNoTracking()
            .Include(p => p.Category).Include(p => p.Brand).Include(p => p.Unit).Include(p => p.StockSummary)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(p => EF.Functions.Like(p.Name, $"%{term}%")
                          || EF.Functions.Like(p.ProductCode, $"%{term}%")
                          || (p.SKU != null && EF.Functions.Like(p.SKU, $"%{term}%"))
                          || p.Barcode == term);
        }

        if (query.CategoryId is { } categoryId)
        {
            q = q.Where(p => p.CategoryId == categoryId);
        }

        if (query.BrandId is { } brandId)
        {
            q = q.Where(p => p.BrandId == brandId);
        }

        if (query.IsActive is { } active)
        {
            q = q.Where(p => p.IsActive == active);
        }

        if (query.LowStockOnly)
        {
            q = q.Where(p => p.StockSummary != null && p.StockSummary.Quantity <= p.MinimumStock);
        }

        if (query.OutOfStockOnly)
        {
            q = q.Where(p => p.StockSummary == null || p.StockSummary.Quantity <= 0);
        }

        if (query.WithExpiryOnly)
        {
            q = q.Where(p => p.ExpiryDateUtc != null);
        }

        var total = await q.CountAsync(ct);

        q = query.SortBy switch
        {
            nameof(Product.Name) => query.Descending ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name),
            nameof(Product.SellingPrice) => query.Descending ? q.OrderByDescending(p => p.SellingPrice) : q.OrderBy(p => p.SellingPrice),
            nameof(Product.PurchasePrice) => query.Descending ? q.OrderByDescending(p => p.PurchasePrice) : q.OrderBy(p => p.PurchasePrice),
            nameof(Product.CreatedAtUtc) => query.Descending ? q.OrderByDescending(p => p.CreatedAtUtc) : q.OrderBy(p => p.CreatedAtUtc),
            _ => q.OrderBy(p => p.ProductCode)
        };

        var items = await q.Skip(query.Skip).Take(query.Take)
            .Select(p => new ProductListItem(
                p.Id,
                p.ProductCode,
                p.Name,
                p.SKU,
                p.Barcode,
                p.Category != null ? p.Category.Name : null,
                p.Brand != null ? p.Brand.Name : null,
                p.Unit != null ? p.Unit.Symbol : null,
                p.PurchasePrice,
                p.SellingPrice,
                p.MRP,
                p.GSTRate,
                p.StockSummary != null ? p.StockSummary.Quantity : 0m,
                p.MinimumStock,
                p.IsActive))
            .ToListAsync(ct);

        return new PagedResult<ProductListItem>(items, total, query.Page, query.Take);
    }

    /// <inheritdoc />
    public async Task<ProductDetailDto?> GetProductAsync(Guid id, CancellationToken ct = default)
    {
        var p = await _db.Products.AsNoTracking().Include(x => x.StockSummary)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (p is null)
        {
            return null;
        }

        return new ProductDetailDto(
            p.Id, p.ProductCode, p.Name, p.SKU, p.Barcode, p.CategoryId, p.BrandId, p.UnitId, p.SupplierId,
            p.PurchasePrice, p.SellingPrice, p.WholesalePrice, p.MRP, p.GSTRate, p.HSNCode,
            p.MinimumStock, p.MaximumStock, p.BatchNumber, p.ExpiryDateUtc, p.ImagePath, p.Description,
            p.IsActive, p.StockSummary?.Quantity ?? 0m);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProductLookupDto>> LookupProductsAsync(string term, int take = 20, CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 100);
        var q = _db.Products.AsNoTracking().Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(term))
        {
            var t = term.Trim();
            q = q.Where(p => EF.Functions.Like(p.Name, $"%{t}%")
                          || EF.Functions.Like(p.ProductCode, $"%{t}%")
                          || (p.SKU != null && EF.Functions.Like(p.SKU, $"%{t}%")));
        }

        return await Project(q.OrderBy(p => p.Name).Take(take)).ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<ProductLookupDto?> FindByBarcodeAsync(string code, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        var t = code.Trim();
        return await Project(_db.Products.AsNoTracking()
            .Where(p => p.IsActive && (p.Barcode == t || p.SKU == t || p.ProductCode == t)))
            .FirstOrDefaultAsync(ct);
    }

    private static IQueryable<ProductLookupDto> Project(IQueryable<Product> q) =>
        q.Select(p => new ProductLookupDto(
            p.Id, p.ProductCode, p.Name, p.Barcode, p.SellingPrice, p.PurchasePrice, p.GSTRate,
            p.StockSummary != null ? p.StockSummary.Quantity : 0m,
            p.Unit != null ? p.Unit.Symbol : null));

    /// <inheritdoc />
    public async Task<Guid> SaveProductAsync(ProductDetailDto product, CancellationToken ct = default)
    {
        ValidateProduct(product);

        var isNew = product.Id == Guid.Empty;
        Product entity;

        if (isNew)
        {
            var code = string.IsNullOrWhiteSpace(product.ProductCode)
                ? await _numbers.GetNextNumberAsync(DocumentType.Product, ct)
                : product.ProductCode.Trim();
            await EnsureCodeUniqueAsync(code, Guid.Empty, ct);

            entity = new Product { ProductCode = code };
            _db.Products.Add(entity);
            _db.StockSummaries.Add(new StockSummary { ProductId = entity.Id, Quantity = 0m });
        }
        else
        {
            entity = await _db.Products.FirstOrDefaultAsync(p => p.Id == product.Id, ct)
                ?? throw new NotFoundException("Product");

            var code = string.IsNullOrWhiteSpace(product.ProductCode) ? entity.ProductCode : product.ProductCode.Trim();
            if (!string.Equals(code, entity.ProductCode, StringComparison.OrdinalIgnoreCase))
            {
                await EnsureCodeUniqueAsync(code, entity.Id, ct);
            }
        }

        await EnsureUniqueAsync(product, entity.Id, ct);

        entity.Name = product.Name.Trim();
        entity.SKU = Blank(product.SKU);
        entity.Barcode = Blank(product.Barcode);
        entity.CategoryId = product.CategoryId;
        entity.BrandId = product.BrandId;
        entity.UnitId = product.UnitId;
        entity.SupplierId = product.SupplierId;
        entity.PurchasePrice = Money.Round(product.PurchasePrice);
        entity.SellingPrice = Money.Round(product.SellingPrice);
        entity.WholesalePrice = Money.Round(product.WholesalePrice);
        entity.MRP = Money.Round(product.MRP);
        entity.GSTRate = product.GSTRate;
        entity.HSNCode = Blank(product.HSNCode);
        entity.MinimumStock = product.MinimumStock;
        entity.MaximumStock = product.MaximumStock;
        entity.BatchNumber = Blank(product.BatchNumber);
        entity.ExpiryDateUtc = product.ExpiryDateUtc;
        entity.ImagePath = Blank(product.ImagePath);
        entity.Description = Blank(product.Description);
        entity.IsActive = product.IsActive;

        await _db.SaveChangesAsync(ct);

        await _audit.RecordAsync(isNew ? "ProductCreated" : "ProductUpdated", "Products",
            nameof(Product), entity.Id.ToString(), null,
            new { entity.ProductCode, entity.Name, entity.SellingPrice, entity.GSTRate }, ct);

        return entity.Id;
    }

    /// <inheritdoc />
    public async Task SetProductActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var entity = await _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException("Product");
        entity.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
        await _audit.RecordAsync(isActive ? "ProductActivated" : "ProductDeactivated", "Products",
            nameof(Product), id.ToString(), null, new { isActive }, ct);
    }

    /// <inheritdoc />
    public async Task DeleteProductAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException("Product");

        var hasHistory = await _db.StockMovements.AnyAsync(m => m.ProductId == id, ct)
                      || await _db.SaleItems.AnyAsync(i => i.ProductId == id, ct)
                      || await _db.PurchaseItems.AnyAsync(i => i.ProductId == id, ct);
        if (hasHistory)
        {
            throw new AppValidationException(
                $"{entity.Name} has stock or transaction history. Deactivate it instead of deleting.");
        }

        entity.IsActive = false;
        entity.DeletedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        await _audit.RecordAsync("ProductDeleted", "Products", nameof(Product), id.ToString(),
            new { entity.ProductCode, entity.Name }, null, ct);
    }

    /// <inheritdoc />
    public async Task<string> PeekNextProductCodeAsync(CancellationToken ct = default) =>
        await _numbers.PeekNextNumberAsync(DocumentType.Product, ct);

    // -------------------------------------------------------------- categories

    /// <inheritdoc />
    public async Task<IReadOnlyList<CategoryListItem>> GetCategoriesAsync(CancellationToken ct = default) =>
        await _db.Categories.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryListItem(
                c.Id, c.Name, c.Description, c.IsActive,
                _db.Products.Count(p => p.CategoryId == c.Id)))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<Guid> SaveCategoryAsync(Guid? id, string name, string? description, bool isActive, CancellationToken ct = default)
    {
        name = RequireName(name, "Category");

        var duplicate = await _db.Categories.AnyAsync(
            c => c.Name == name && (id == null || c.Id != id), ct);
        if (duplicate)
        {
            throw new AppValidationException($"A category named \"{name}\" already exists.");
        }

        Category entity;
        if (id is { } existingId)
        {
            entity = await _db.Categories.FirstOrDefaultAsync(c => c.Id == existingId, ct)
                ?? throw new NotFoundException("Category");
            entity.Name = name;
            entity.Description = Blank(description);
            entity.IsActive = isActive;
        }
        else
        {
            entity = new Category { Name = name, Description = Blank(description), IsActive = isActive };
            _db.Categories.Add(entity);
        }

        await _db.SaveChangesAsync(ct);
        await _audit.RecordAsync(id is null ? "CategoryCreated" : "CategoryUpdated", "Products",
            nameof(Category), entity.Id.ToString(), null, new { entity.Name, entity.IsActive }, ct);
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task SetCategoryActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var entity = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException("Category");
        entity.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
        await _audit.RecordAsync(isActive ? "CategoryActivated" : "CategoryDeactivated", "Products",
            nameof(Category), id.ToString(), null, new { isActive }, ct);
    }

    // ------------------------------------------------------------------ brands

    /// <inheritdoc />
    public async Task<IReadOnlyList<BrandListItem>> GetBrandsAsync(CancellationToken ct = default) =>
        await _db.Brands.AsNoTracking()
            .OrderBy(b => b.Name)
            .Select(b => new BrandListItem(
                b.Id, b.Name, b.Description, b.IsActive,
                _db.Products.Count(p => p.BrandId == b.Id)))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<Guid> SaveBrandAsync(Guid? id, string name, string? description, bool isActive, CancellationToken ct = default)
    {
        name = RequireName(name, "Brand");

        var duplicate = await _db.Brands.AnyAsync(
            b => b.Name == name && (id == null || b.Id != id), ct);
        if (duplicate)
        {
            throw new AppValidationException($"A brand named \"{name}\" already exists.");
        }

        Brand entity;
        if (id is { } existingId)
        {
            entity = await _db.Brands.FirstOrDefaultAsync(b => b.Id == existingId, ct)
                ?? throw new NotFoundException("Brand");
            entity.Name = name;
            entity.Description = Blank(description);
            entity.IsActive = isActive;
        }
        else
        {
            entity = new Brand { Name = name, Description = Blank(description), IsActive = isActive };
            _db.Brands.Add(entity);
        }

        await _db.SaveChangesAsync(ct);
        await _audit.RecordAsync(id is null ? "BrandCreated" : "BrandUpdated", "Products",
            nameof(Brand), entity.Id.ToString(), null, new { entity.Name, entity.IsActive }, ct);
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task SetBrandActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var entity = await _db.Brands.FirstOrDefaultAsync(b => b.Id == id, ct)
            ?? throw new NotFoundException("Brand");
        entity.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
        await _audit.RecordAsync(isActive ? "BrandActivated" : "BrandDeactivated", "Products",
            nameof(Brand), id.ToString(), null, new { isActive }, ct);
    }

    // ------------------------------------------------------------------- units

    /// <inheritdoc />
    public async Task<IReadOnlyList<UnitListItem>> GetUnitsAsync(CancellationToken ct = default) =>
        await _db.Units.AsNoTracking()
            .OrderBy(u => u.Name)
            .Select(u => new UnitListItem(
                u.Id, u.Name, u.Symbol, u.DecimalAllowed, u.IsSystem, u.IsActive,
                _db.Products.Count(p => p.UnitId == u.Id)))
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<Guid> SaveUnitAsync(Guid? id, string name, string symbol, bool decimalAllowed, bool isActive, CancellationToken ct = default)
    {
        name = RequireName(name, "Unit");
        symbol = RequireName(symbol, "Unit symbol").ToUpperInvariant();

        var duplicate = await _db.Units.AnyAsync(
            u => (u.Name == name || u.Symbol == symbol) && (id == null || u.Id != id), ct);
        if (duplicate)
        {
            throw new AppValidationException($"A unit named \"{name}\" or with symbol \"{symbol}\" already exists.");
        }

        Unit entity;
        if (id is { } existingId)
        {
            entity = await _db.Units.FirstOrDefaultAsync(u => u.Id == existingId, ct)
                ?? throw new NotFoundException("Unit");
            entity.Name = name;
            entity.Symbol = symbol;
            entity.DecimalAllowed = decimalAllowed;
            entity.IsActive = isActive;
        }
        else
        {
            entity = new Unit { Name = name, Symbol = symbol, DecimalAllowed = decimalAllowed, IsActive = isActive };
            _db.Units.Add(entity);
        }

        await _db.SaveChangesAsync(ct);
        await _audit.RecordAsync(id is null ? "UnitCreated" : "UnitUpdated", "Products",
            nameof(Unit), entity.Id.ToString(), null, new { entity.Name, entity.Symbol, entity.IsActive }, ct);
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task SetUnitActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var entity = await _db.Units.FirstOrDefaultAsync(u => u.Id == id, ct)
            ?? throw new NotFoundException("Unit");
        entity.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
        await _audit.RecordAsync(isActive ? "UnitActivated" : "UnitDeactivated", "Products",
            nameof(Unit), id.ToString(), null, new { isActive }, ct);
    }

    // --------------------------------------------------------------- tax rates

    /// <inheritdoc />
    public async Task<IReadOnlyList<TaxRate>> GetTaxRatesAsync(bool includeInactive = false, CancellationToken ct = default) =>
        await _db.TaxRates.AsNoTracking()
            .Where(t => includeInactive || t.IsActive)
            .OrderBy(t => t.RatePercent)
            .ToListAsync(ct);

    /// <inheritdoc />
    public async Task<Guid> SaveTaxRateAsync(Guid? id, string name, decimal ratePercent, bool isDefault, bool isActive, CancellationToken ct = default)
    {
        name = RequireName(name, "Tax rate");

        if (ratePercent is < 0 or > 100)
        {
            throw new AppValidationException("GST rate must be between 0 and 100 percent.");
        }

        var duplicate = await _db.TaxRates.AnyAsync(
            t => t.RatePercent == ratePercent && (id == null || t.Id != id), ct);
        if (duplicate)
        {
            throw new AppValidationException($"A tax rate of {ratePercent}% already exists.");
        }

        TaxRate entity;
        if (id is { } existingId)
        {
            entity = await _db.TaxRates.FirstOrDefaultAsync(t => t.Id == existingId, ct)
                ?? throw new NotFoundException("Tax rate");
            entity.Name = name;
            entity.RatePercent = ratePercent;
            entity.IsDefault = isDefault;
            entity.IsActive = isActive;
        }
        else
        {
            entity = new TaxRate { Name = name, RatePercent = ratePercent, IsDefault = isDefault, IsActive = isActive };
            _db.TaxRates.Add(entity);
        }

        // Only one rate may be the suggested default.
        if (isDefault)
        {
            var others = await _db.TaxRates.Where(t => t.Id != entity.Id && t.IsDefault).ToListAsync(ct);
            foreach (var other in others)
            {
                other.IsDefault = false;
            }
        }

        await _db.SaveChangesAsync(ct);
        await _audit.RecordAsync(id is null ? "TaxRateCreated" : "TaxRateUpdated", "Settings",
            nameof(TaxRate), entity.Id.ToString(), null, new { entity.Name, entity.RatePercent, entity.IsActive }, ct);
        return entity.Id;
    }

    // ---------------------------------------------------------------- helpers

    private static string RequireName(string? value, string label)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new AppValidationException($"{label} name is required.");
        }

        return trimmed;
    }

    private static string? Blank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void ValidateProduct(ProductDetailDto product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
        {
            throw new AppValidationException("Product name is required.");
        }

        if (product.SellingPrice < 0 || product.PurchasePrice < 0 || product.MRP < 0)
        {
            throw new AppValidationException("Product prices cannot be negative.");
        }

        if (product.GSTRate is < 0 or > 100)
        {
            throw new AppValidationException("GST rate must be between 0 and 100 percent.");
        }

        if (product.MinimumStock < 0)
        {
            throw new AppValidationException("Minimum stock cannot be negative.");
        }
    }

    /// <summary>Rejects duplicate product codes (case-insensitive).</summary>
    private async Task EnsureCodeUniqueAsync(string code, Guid excludingId, CancellationToken ct)
    {
        var exists = await _db.Products.AnyAsync(
            p => p.ProductCode == code && p.Id != excludingId, ct);
        if (exists)
        {
            throw new AppValidationException($"Product code \"{code}\" is already in use.");
        }
    }

    /// <summary>Rejects duplicate SKU and barcode values, ignoring null/blank entries.</summary>
    private async Task EnsureUniqueAsync(ProductDetailDto product, Guid excludingId, CancellationToken ct)
    {
        var sku = Blank(product.SKU);
        if (sku is not null)
        {
            var exists = await _db.Products.AnyAsync(
                p => p.SKU == sku && p.Id != excludingId, ct);
            if (exists)
            {
                throw new AppValidationException($"SKU \"{sku}\" is already in use.");
            }
        }

        var barcode = Blank(product.Barcode);
        if (barcode is not null)
        {
            var exists = await _db.Products.AnyAsync(
                p => p.Barcode == barcode && p.Id != excludingId, ct);
            if (exists)
            {
                throw new AppValidationException($"Barcode \"{barcode}\" is already in use.");
            }
        }
    }
}