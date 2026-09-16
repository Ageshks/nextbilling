namespace BizStock.Application.Common.Licensing;

public interface ILicenseService
{
    Task<LicenseActivationResult> ActivateAsync(string licenseKey, string? installationId = null, string? deviceName = null, CancellationToken cancellationToken = default);

    Task<LicenseValidationResult> ValidateAsync(string? installationId = null, CancellationToken cancellationToken = default);

    Task<LicenseActivationResult> DeactivateAsync(string? installationId = null, CancellationToken cancellationToken = default);

    LicenseInfo? GetCurrentLicense();

    bool HasFeature(string featureCode);

    bool IsLicenseActive();
}
