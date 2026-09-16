using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BizStock.Application.Common.Licensing;

public static class LicenseTokenService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static string Create(LicenseTokenPayload payload, string publicKey)
    {
        var serialized = JsonSerializer.Serialize(payload, JsonOptions);
        var payloadBytes = Encoding.UTF8.GetBytes(serialized);
        var privateKey = LicenseSigningKeyPair.GetOrCreate();
        var signature = privateKey.SignData(payloadBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var encodedPayload = Convert.ToBase64String(payloadBytes);
        var encodedSignature = Convert.ToBase64String(signature);
        return $"{encodedPayload}.{encodedSignature}";
    }

    public static bool TryValidate(string token, string publicKey, out LicenseTokenPayload? payload)
    {
        payload = null;

        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        try
        {
            var parts = token.Split('.', 2);
            if (parts.Length != 2)
            {
                return false;
            }

            var payloadBytes = Convert.FromBase64String(parts[0]);
            var signature = Convert.FromBase64String(parts[1]);

            var rsa = CreatePublicKeyFromString(publicKey);
            var isValid = rsa.VerifyData(payloadBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            if (!isValid)
            {
                return false;
            }

            var json = Encoding.UTF8.GetString(payloadBytes);
            payload = JsonSerializer.Deserialize<LicenseTokenPayload>(json, JsonOptions);
            return payload is not null;
        }
        catch
        {
            return false;
        }
    }

    private static RSA CreatePublicKeyFromString(string publicKey)
    {
        var rsa = RSA.Create();
        var xml = Encoding.UTF8.GetString(Convert.FromBase64String(publicKey));
        rsa.FromXmlString(xml);
        return rsa;
    }
}
