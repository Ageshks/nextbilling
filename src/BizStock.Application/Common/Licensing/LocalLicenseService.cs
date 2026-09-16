namespace BizStock.Application.Common.Licensing;

public class LocalLicenseService : ILicenseService
{
    private LicenseInfo? _license;

    public Task<LicenseActivationResult> ActivateAsync(string licenseKey, string? installationId = null, string? deviceName = null, CancellationToken cancellationToken = default)
    {
        if (!LicenseKeyGenerator.IsValid(licenseKey))
        {
            return Task.FromResult(new LicenseActivationResult(false, "The license key is invalid."));
        }

        var info = new LicenseInfo(
            LicenseId: "local-license",
            LicenseKey: licenseKey.Trim(),
            Plan: "Professional",
            Status: LicenseStatus.Active,
            ExpiresAtUtc: DateTimeOffset.UtcNow.AddDays(30),
            IsTrial: false,
            MaxDevices: 3,
            Features: new[]
            {
                FeatureCodes.Billing,
                FeatureCodes.Inventory,
                FeatureCodes.Sales,
                FeatureCodes.Purchases,
                FeatureCodes.Customers,
                FeatureCodes.Suppliers,
                FeatureCodes.Payroll,
                FeatureCodes.Finance,
                FeatureCodes.Accounting,
                FeatureCodes.Reports
            },
            InstallationId: installationId ?? "install-local");

        _license = info;
        return Task.FromResult(new LicenseActivationResult(true, "License activated successfully.", info));
    }

    public Task<LicenseValidationResult> ValidateAsync(string? installationId = null, CancellationToken cancellationToken = default)
    {
        if (_license is null)
        {
            return Task.FromResult(new LicenseValidationResult(false, "No license is active on this device."));
        }

        if (_license.Status != LicenseStatus.Active)
        {
            return Task.FromResult(new LicenseValidationResult(false, "The license is not active.", _license));
        }

        if (_license.ExpiresAtUtc.HasValue && _license.ExpiresAtUtc.Value < DateTimeOffset.UtcNow)
        {
            return Task.FromResult(new LicenseValidationResult(false, "The license has expired.", _license));
        }

        return Task.FromResult(new LicenseValidationResult(true, "License is valid.", _license));
    }

    public Task<LicenseActivationResult> DeactivateAsync(string? installationId = null, CancellationToken cancellationToken = default)
    {
        if (_license is null)
        {
            return Task.FromResult(new LicenseActivationResult(false, "No active license to deactivate."));
        }

        _license = null;
        return Task.FromResult(new LicenseActivationResult(true, "License deactivated successfully."));
    }

    public LicenseInfo? GetCurrentLicense() => _license;

    public bool HasFeature(string featureCode)
    {
        return _license is not null && _license.HasFeature(featureCode);
    }

    public bool IsLicenseActive() => _license is not null && _license.Status == LicenseStatus.Active && (!(_license.ExpiresAtUtc.HasValue && _license.ExpiresAtUtc.Value < DateTimeOffset.UtcNow));
}
