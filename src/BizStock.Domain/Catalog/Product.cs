using BizStock.Domain.Common;
using BizStock.Domain.Catalog;
using BizStock.Domain.Inventory;
using BizStock.Domain.Partners;

namespace BizStock.Domain.Catalog;

/// <summary>Tradeable product with pricing, GST and stock configuration.</summary>
public class Product : BaseEntity, ISoftDeletable, IActivatable, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique product code, e.g. P0001.</summary>
    public string ProductCode { get; set; } = default!;

    /// <summary>Product name.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Unique stock keeping unit.</summary>
    public string? SKU { get; set; }

    /// <summary>Unique barcode (EAN/UPC or code128). USB scanners resolve through this.</summary>
    public string? Barcode { get; set; }

    /// <summary>Category.</summary>
    public Guid? CategoryId { get; set; }

    /// <summary>Brand.</summary>
    public Guid? BrandId { get; set; }

    /// <summary>Unit of measure.</summary>
    public Guid? UnitId { get; set; }

    /// <summary>Purchase (cost) price used for stock valuation and COGS.</summary>
    public decimal PurchasePrice { get; set; }

    /// <summary>Default retail selling price.</summary>
    public decimal SellingPrice { get; set; }

    /// <summary>Wholesale price.</summary>
    public decimal WholesalePrice { get; set; }

    /// <summary>Maximum retail price.</summary>
    public decimal MRP { get; set; }

    /// <summary>Applicable GST rate percent.</summary>
    public decimal GSTRate { get; set; }

    /// <summary>HSN classification code for GST reporting.</summary>
    public string? HSNCode { get; set; }

    /// <summary>Stock level at which the product is flagged low.</summary>
    public decimal MinimumStock { get; set; }

    /// <summary>Optional maximum stock hint.</summary>
    public decimal? MaximumStock { get; set; }

    /// <summary>Preferred supplier.</summary>
    public Guid? SupplierId { get; set; }

    /// <summary>Optional batch number.</summary>
    public string? BatchNumber { get; set; }

    /// <summary>Optional expiry date.</summary>
    public DateTime? ExpiryDateUtc { get; set; }

    /// <summary>Product image path.</summary>
    public string? ImagePath { get; set; }

    /// <summary>Free-text description.</summary>
    public string? Description { get; set; }

    /// <summary>Whether the product is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Soft-delete timestamp (UTC).</summary>
    public DateTime? DeletedAtUtc { get; set; }

    /// <summary>Category navigation.</summary>
    public Category? Category { get; set; }

    /// <summary>Brand navigation.</summary>
    public Brand? Brand { get; set; }

    /// <summary>Unit navigation.</summary>
    public Unit? Unit { get; set; }

    /// <summary>Preferred supplier navigation.</summary>
    public Supplier? PreferredSupplier { get; set; }

    /// <summary>Live stock balance row.</summary>
    public StockSummary? StockSummary { get; set; }
}
