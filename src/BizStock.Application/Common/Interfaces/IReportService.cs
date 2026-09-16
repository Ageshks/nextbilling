namespace BizStock.Application.Common.Interfaces;

/// <summary>Reporting queries for dashboards and report screens.</summary>
public interface IReportService
{
    /// <summary>Report period.</summary>
    public record PeriodReport(DateOnly From, DateOnly To);

    /// <summary>Sales summary numbers for a period.</summary>
    Task<SalesSummaryDto> GetSalesSummaryAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

    /// <summary>Daily sales for a period.</summary>
    Task<IReadOnlyList<DailySalesDto>> GetDailySalesAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

    /// <summary>Top products by revenue for a period.</summary>
    Task<IReadOnlyList<TopProductDto>> GetTopProductsAsync(DateTime fromUtc, DateTime toUtc, int take, CancellationToken ct = default);

    /// <summary>Sales grouped by GST slab (GSTR-style summary).</summary>
    Task<IReadOnlyList<GstSlabSummaryDto>> GetGstSummaryAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

    /// <summary>Stock valuation snapshot.</summary>
    Task<IReadOnlyList<StockValuationDto>> GetStockValuationAsync(CancellationToken ct = default);

    /// <summary>Profit &amp; loss figures for a period.</summary>
    Task<ProfitAndLossDto> GetProfitAndLossAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
}

/// <summary>Headline sales numbers.</summary>
public record SalesSummaryDto(int InvoiceCount, decimal GrossSales, decimal Discounts, decimal TaxCollected, decimal NetSales);

/// <summary>One day's sales.</summary>
public record DailySalesDto(DateOnly Date, int InvoiceCount, decimal NetSales, decimal Cash, decimal Upi, decimal Card, decimal Credit);

/// <summary>Best-selling product.</summary>
public record TopProductDto(string ProductCode, string ProductName, decimal QuantitySold, decimal Revenue, decimal Profit);

/// <summary>GST slab totals for a period.</summary>
public record GstSlabSummaryDto(decimal RatePercent, decimal TaxableAmount, decimal CGST, decimal SGST, decimal IGST, decimal TotalTax);

/// <summary>Stock valuation row.</summary>
public record StockValuationDto(string ProductCode, string ProductName, decimal Quantity, decimal AverageCost, decimal StockValue, decimal SellingValue);

/// <summary>P&amp;L figures.</summary>
public record ProfitAndLossDto(decimal Revenue, decimal COGS, decimal GrossProfit, decimal Expenses, decimal NetProfit);
