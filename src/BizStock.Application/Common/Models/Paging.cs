namespace BizStock.Application.Common.Models;

/// <summary>Paged query envelope. All list screens page at the database level.</summary>
public class PageRequest
{
    /// <summary>One-based page number.</summary>
    public int Page { get; set; } = 1;

    /// <summary>Rows per page (clamped by services to a safe maximum).</summary>
    public int PageSize { get; set; } = 50;

    /// <summary>Free-text search term.</summary>
    public string? Search { get; set; }

    /// <summary>Column/property to sort by.</summary>
    public string? SortBy { get; set; }

    /// <summary>Whether the sort is descending.</summary>
    public bool Descending { get; set; }

    /// <summary>Zero-based skip count derived from page and size.</summary>
    public int Skip => Math.Max(0, (Math.Max(1, Page) - 1) * Math.Clamp(PageSize, 1, 500));

    /// <summary>Effective page size, clamped to 1..500.</summary>
    public int Take => Math.Clamp(PageSize, 1, 500);
}

/// <summary>Paged result set.</summary>
/// <typeparam name="T">Row projection type.</typeparam>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    /// <summary>Total number of pages available.</summary>
    public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Creates an empty page.</summary>
    public static PagedResult<T> Empty(int page = 1, int pageSize = 50) => new([], 0, page, pageSize);
}