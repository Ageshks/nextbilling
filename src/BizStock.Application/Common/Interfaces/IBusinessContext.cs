using BizStock.Domain.Business;

namespace BizStock.Application.Common.Interfaces;

/// <summary>Business profile + settings access.</summary>
public interface IBusinessContext
{
    /// <summary>Business profile, or null before setup.</summary>
    Task<Business?> GetBusinessAsync(CancellationToken ct = default);

    /// <summary>Creates/updates the business profile (setup wizard).</summary>
    Task<Business> SaveBusinessAsync(Business business, CancellationToken ct = default);

    /// <summary>Operational settings (creates defaults on first access).</summary>
    Task<BusinessSettings> GetSettingsAsync(CancellationToken ct = default);

    /// <summary>Persists settings changes.</summary>
    Task SaveSettingsAsync(BusinessSettings settings, CancellationToken ct = default);
}
