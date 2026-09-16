using BizStock.Domain.Common;

namespace BizStock.Domain.Business;

/// <summary>Business profile: identity, GST registration and financial-year configuration.</summary>
public class Business : BaseEntity
{
    /// <summary>Trading name shown across the application and invoices.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Registered legal name.</summary>
    public string? LegalName { get; set; }

    /// <summary>GST Identification Number.</summary>
    public string? GSTIN { get; set; }

    /// <summary>PAN.</summary>
    public string? PAN { get; set; }

    /// <summary>Contact email.</summary>
    public string? Email { get; set; }

    /// <summary>Contact phone.</summary>
    public string? Phone { get; set; }

    /// <summary>Website.</summary>
    public string? Website { get; set; }

    /// <summary>Street address.</summary>
    public string? AddressLine { get; set; }

    /// <summary>City.</summary>
    public string? City { get; set; }

    /// <summary>State name.</summary>
    public string? State { get; set; }

    /// <summary>State code (e.g. 27 for Maharashtra). Drives intra/inter-state GST.</summary>
    public string? StateCode { get; set; }

    /// <summary>PIN code.</summary>
    public string? Pincode { get; set; }

    /// <summary>Logo image path.</summary>
    public string? LogoPath { get; set; }

    /// <summary>Currency code. Default INR.</summary>
    public string Currency { get; set; } = "INR";

    /// <summary>Whether the first-run setup wizard has been completed.</summary>
    public bool IsSetupCompleted { get; set; }

    /// <summary>Financial year start month (1-12). Indian default: 4 (April).</summary>
    public int FinancialYearStartMonth { get; set; } = 4;

    /// <summary>Financial year start day (1-28). Indian default: 1.</summary>
    public int FinancialYearStartDay { get; set; } = 1;
}

/// <summary>One-to-one operational settings for the business. Engine-critical flags live here.</summary>
public class BusinessSettings : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    // ---- Inventory ----
    /// <summary>Whether selling below zero stock is allowed. Default false.</summary>
    public bool AllowNegativeStock { get; set; }

    /// <summary>Default low-stock threshold used when a product has none.</summary>
    public decimal LowStockThreshold { get; set; } = 5;

    /// <summary>Whether batch tracking is enabled.</summary>
    public bool EnableBatch { get; set; }

    /// <summary>Whether expiry tracking is enabled.</summary>
    public bool EnableExpiry { get; set; }

    // ---- Invoicing ----
    /// <summary>Invoice number prefix, e.g. "INV".</summary>
    public string InvoicePrefix { get; set; } = "INV";

    /// <summary>Whether invoice numbers reset each financial year, e.g. INV-2026-000001.</summary>
    public bool InvoiceNumberResetsYearly { get; set; } = true;

    /// <summary>Invoice footer text.</summary>
    public string? InvoiceFooter { get; set; }

    /// <summary>Invoice terms &amp; conditions.</summary>
    public string? InvoiceTerms { get; set; }

    /// <summary>Whether totals are rounded to the nearest rupee.</summary>
    public bool RoundOffEnabled { get; set; } = true;

    // ---- Printing ----
    /// <summary>Thermal paper width in mm (58 or 80).</summary>
    public int ThermalPaperWidth { get; set; } = 80;

    /// <summary>Number of copies to print.</summary>
    public int PrintCopies { get; set; } = 1;

    /// <summary>Whether invoices print automatically after saving.</summary>
    public bool AutoPrintInvoice { get; set; }

    /// <summary>Default printer name.</summary>
    public string? DefaultPrinter { get; set; }

    // ---- Payroll (configuration reserved for Phase 9) ----
    /// <summary>Standard working days per month used by payroll.</summary>
    public int WorkingDaysPerMonth { get; set; } = 26;

    // ---- Backup ----
    /// <summary>Whether scheduled backups are enabled.</summary>
    public bool BackupEnabled { get; set; } = true;

    /// <summary>Backup frequency in days.</summary>
    public int BackupFrequencyDays { get; set; } = 1;

    /// <summary>Backup directory. Empty = default app-data location.</summary>
    public string? BackupLocation { get; set; }

    /// <summary>Last successful backup timestamp (UTC).</summary>
    public DateTime? LastBackupAtUtc { get; set; }
}

/// <summary>Configurable financial year. Transactions are validated against the current one.</summary>
public class FinancialYear : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Display name, e.g. "FY 2026-27".</summary>
    public string Name { get; set; } = default!;

    /// <summary>Inclusive start date.</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>Inclusive end date.</summary>
    public DateOnly EndDate { get; set; }

    /// <summary>Whether this is the active financial year.</summary>
    public bool IsCurrent { get; set; }

    /// <summary>Closed years reject new postings.</summary>
    public bool IsClosed { get; set; }
}
