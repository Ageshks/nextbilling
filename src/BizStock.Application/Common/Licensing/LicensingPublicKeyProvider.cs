using System.Security.Cryptography;
using System.Text;

namespace BizStock.Application.Common.Licensing;

public static class LicensingPublicKeyProvider
{
    private static readonly Lazy<string> PublicKey = new(CreatePublicKey);

    public static string GetPublicKey() => PublicKey.Value;

    private static string CreatePublicKey()
    {
        var rsa = LicenseSigningKeyPair.GetOrCreate();
        var publicXml = rsa.ToXmlString(false);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(publicXml));
    }
}

internal static class LicenseSigningKeyPair
{
    private static readonly Lazy<RSA> Key = new(() =>
    {
        var rsa = RSA.Create(2048);
        rsa.KeySize = 2048;
        return rsa;
    });

    public static RSA GetOrCreate() => Key.Value;
}
