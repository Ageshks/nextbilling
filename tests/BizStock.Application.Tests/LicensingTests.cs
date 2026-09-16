using System.Text;
using BizStock.Application.Common.Licensing;

namespace BizStock.Application.Tests;

public class LicensingTests
{
    [Fact]
    public void LicenseKeyGenerator_CreatesFormatAndUniqueKeys()
    {
        var key1 = LicenseKeyGenerator.Generate();
        var key2 = LicenseKeyGenerator.Generate();

        Assert.NotEqual(key1, key2);
        Assert.Matches("^BIZSTOCK-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$", key1);
        Assert.Matches("^BIZSTOCK-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$", key2);
    }

    [Fact]
    public void SignedLicenseToken_RejectsTampering()
    {
        var publicKey = LicensingPublicKeyProvider.GetPublicKey();
        var token = LicenseTokenService.Create(new LicenseTokenPayload
        {
            LicenseId = "license-1",
            LicenseKey = "BIZSTOCK-ABCD-EFGH-IJKL-MNOP",
            Plan = "Professional",
            Status = LicenseStatus.Active,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30),
            Features = [FeatureCodes.Billing, FeatureCodes.Inventory],
            InstallationId = "install-123"
        }, publicKey);

        var parts = token.Split('.', 2);
        var payload = Encoding.UTF8.GetString(Convert.FromBase64String(parts[0]));
        var tamperedPayload = payload.Replace("\"license-1\"", "\"license-2\"");
        var tampered = Convert.ToBase64String(Encoding.UTF8.GetBytes(tamperedPayload)) + "." + parts[1];

        Assert.False(LicenseTokenService.TryValidate(tampered, publicKey, out _));
    }

    [Fact]
    public async Task LicenseActivation_RejectsInvalidKey()
    {
        var service = new LocalLicenseService();

        var result = await service.ActivateAsync("INVALID-KEY");

        Assert.False(result.Success);
        Assert.Contains("invalid", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}
