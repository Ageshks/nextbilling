namespace BizStock.Application.Common.Licensing;

public sealed record LicenseInfo(
    string LicenseId,
    string LicenseKey,
    string Plan,
    LicenseStatus Status,
    DateTimeOffset? ExpiresAtUtc,
    bool IsTrial,
    int MaxDevices,
    IReadOnlyList<string> Features,
    string InstallationId)
{
    public bool HasFeature(string featureCode) => Features.Any(feature => string.Equals(feature, featureCode, StringComparison.OrdinalIgnoreCase));
}

public sealed record LicenseActivationResult(bool Success, string Message, LicenseInfo? License = null);

public sealed record LicenseValidationResult(bool Success, string Message, LicenseInfo? License = null);

public sealed record LicenseTokenPayload
{
    public string LicenseId { get; init; } = string.Empty;
    public string LicenseKey { get; init; } = string.Empty;
    public string Plan { get; init; } = string.Empty;
    public LicenseStatus Status { get; init; }
    public DateTimeOffset ExpiresAtUtc { get; init; }
    public bool IsTrial { get; init; }
    public int MaxDevices { get; init; }
    public IReadOnlyList<string> Features { get; init; } = Array.Empty<string>();
    public string InstallationId { get; init; } = string.Empty;
    public DateTimeOffset IssuedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public string TokenVersion { get; init; } = "1";
}
