using Shared.Common.Abstract;

public class Password : ValueObject
{
    public string Hash { get; private set; }

    private Password() { } // For EF Core

    public static Password Create(string plainTextPassword)
    {
        // Implement password hashing logic
        string hash = HashPassword(plainTextPassword);
        return new Password { Hash = hash };
    }

    public bool Verify(string plainTextPassword)
    {
        // Implement password verification logic
        return VerifyPassword(plainTextPassword, Hash);
    }

    private static string HashPassword(string password)
    {
        // Implement secure password hashing (e.g., using BCrypt)
        throw new NotImplementedException();
    }

    private static bool VerifyPassword(string password, string hash)
    {
        // Implement secure password verification
        throw new NotImplementedException();
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Hash;
    }
}