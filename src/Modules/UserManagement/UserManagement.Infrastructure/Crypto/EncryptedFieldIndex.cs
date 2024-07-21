
using System.Security.Cryptography;
using System.Text;

namespace UserManagement.Infrastructure.Crypto;
public class EncryptedFieldIndex
{
    public Guid Id { get; set; }
    public string FieldName { get; set; }
    public string HashedValue { get; set; }
    public Guid EntityId { get; set; }

    public static string HashValue(string value)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}