using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Business;
using Microsoft.EntityFrameworkCore;

using BizStock.Infrastructure.Persistence;
namespace BizStock.Infrastructure.Services;

/// <summary>Business profile and settings accessor with change tracking.</summary>
public class BusinessContext : IBusinessContext
{
    private readonly AppDbContext _db;
    private Business? _business;
    private BusinessSettings? _settings;

    /// <summary>Creates the business context.</summary>
    public BusinessContext(AppDbContext db) => _db = db;

    /// <inheritdoc />
    public async Task<Business?> GetBusinessAsync(CancellationToken ct = default) =>
        _business ??= await _db.Businesses.OrderBy(b => b.CreatedAtUtc).FirstOrDefaultAsync(ct);

    /// <inheritdoc />
    public async Task<Business> SaveBusinessAsync(Business business, CancellationToken ct = default)
    {
        var existing = await GetBusinessAsync(ct);
        if (existing is null)
        {
            _db.Businesses.Add(business);
            _business = business;
        }
        else
        {
            _db.Entry(existing).CurrentValues.SetValues(business);
        }
        await _db.SaveChangesAsync(ct);
        _business = existing ?? business;
        return _business;
    }

    /// <inheritdoc />
    public async Task<BusinessSettings> GetSettingsAsync(CancellationToken ct = default)
    {
        if (_settings is not null)
        {
            return _settings;
        }

        _settings = await _db.BusinessSettings.FirstOrDefaultAsync(ct);
        if (_settings is null)
        {
            _settings = new BusinessSettings();
            _db.BusinessSettings.Add(_settings);
            await _db.SaveChangesAsync(ct);
        }
        return _settings;
    }

    /// <inheritdoc />
    public async Task SaveSettingsAsync(BusinessSettings settings, CancellationToken ct = default)
    {
        var tracked = await GetSettingsAsync(ct);
        _db.Entry(tracked).CurrentValues.SetValues(settings);
        await _db.SaveChangesAsync(ct);
        _settings = tracked;
    }
}
