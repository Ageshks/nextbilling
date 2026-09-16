using System.Security.Cryptography;

namespace BizStock.Infrastructure.Security;

/// <summary>
/// PBKDF2-SHA256 password hasher. Hash format:
/// PBKDF2.v1.{iterations}.{base64salt}.{base64subkey}
/// 210,000 iterations per current OWASP guidance.
/// </summary>
public class PasswordHasher
{
    private const int SaltSize = 32;
    private const int KeySize = 32;
    private const int Iterations = 210_000;

    /// <summary>Hashes a password into the versioned storage format.</summary>
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return $"PBKDF2.v1.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    /// <summary>Verifies a password against a stored hash in constant time.</summary>
    public bool Verify(string password, string storedHash)
    {
        try
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 5 || parts[0] != "PBKDF2" || parts[1] != "v1")
            {
                return false;
            }

            var iterations = int.Parse(parts[2]);
            var salt = Convert.FromBase64String(parts[3]);
            var expected = Convert.FromBase64String(parts[4]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
