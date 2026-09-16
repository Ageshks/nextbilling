using System.Security.Cryptography;

namespace BizStock.Application.Common.Licensing;

public static class LicenseKeyGenerator
{
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

    public static string Generate(string prefix = "BIZSTOCK")
    {
        var parts = new[]
        {
            CreateSegment(),
            CreateSegment(),
            CreateSegment(),
            CreateSegment(),
        };

        return $"{prefix}-{parts[0]}-{parts[1]}-{parts[2]}-{parts[3]}";
    }

    public static bool IsValid(string? licenseKey)
    {
        if (string.IsNullOrWhiteSpace(licenseKey))
        {
            return false;
        }

        var normalized = licenseKey.Trim();
        if (!normalized.Contains('-'))
        {
            return false;
        }

        var parts = normalized.Split('-');
        if (parts.Length != 5)
        {
            return false;
        }

        return parts[0].Equals("BIZSTOCK", StringComparison.OrdinalIgnoreCase)
            && parts.Skip(1).All(part => part.Length == 4 && part.All(char.IsLetterOrDigit));
    }

    private static string CreateSegment()
    {
        var chars = new char[4];
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = GetRandomChar();
        }

        return new string(chars);
    }

    private static char GetRandomChar()
    {
        var bytes = new byte[1];
        Rng.GetBytes(bytes);
        var value = bytes[0] % 36;
        return value < 10 ? (char)('0' + value) : (char)('A' + value - 10);
    }
}
