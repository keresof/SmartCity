using System.Security.Cryptography;
using Shared.Common.Abstract;

namespace UserManagement.Domain.ValueObjects;
public class Password : ValueObject
{
    public string Hash { get; private set; }

    private Password() { } // For EF Core

    public static Password Create(string plainTextPassword)
    {
        // Ensure password is not null or empty
        if (string.IsNullOrEmpty(plainTextPassword))
            throw new ArgumentException("Password cannot be null or empty");
        // Implement password hashing logic
        string hash = HashPassword(plainTextPassword);
        return new Password { Hash = hash };
    }

    public bool Verify(string plainTextPassword)
    {
        if (string.IsNullOrEmpty(plainTextPassword))
            return false;
        // Implement password verification logic
        return VerifyPassword(plainTextPassword, Hash);
    }

    private static string HashPassword(string password)
    {
        var nonce = RandomNumberGenerator.GetBytes(16);
        var pbkdf2 = new Rfc2898DeriveBytes(password, nonce, 10000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(hash) + '.' + Convert.ToBase64String(nonce);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2)
            return false;

        var hash = Convert.FromBase64String(parts[0]);
        var nonce = Convert.FromBase64String(parts[1]);

        var pbkdf2 = new Rfc2898DeriveBytes(password, nonce, 10000, HashAlgorithmName.SHA256);
        var testHash = pbkdf2.GetBytes(32);

        return hash.SequenceEqual(testHash);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Hash;
    }
}