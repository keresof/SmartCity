using System.Security.Cryptography;
using System.Text;
using Shared.Common.Abstract;
using Shared.Common.Interfaces;

namespace Shared.Common.ValueObjects;
public class EncryptedField : ValueObject
    {
        public string EncryptedValue { get; private set; }
        public string HashedValue { get; private set; }

        private EncryptedField(string encryptedValue, string hashedValue)
        {
            EncryptedValue = encryptedValue;
            HashedValue = hashedValue;
        }

        public static EncryptedField Create(string plainText, IEncryptionService encryptionService)
        {
            var encryptedValue = encryptionService.Encrypt(plainText);
            var hashedValue = HashValue(plainText);
            return new EncryptedField(encryptedValue, hashedValue);
        }

        public string Decrypt(IEncryptionService encryptionService)
        {
            return encryptionService.Decrypt(EncryptedValue);
        }

        private static string HashValue(string value)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return EncryptedValue;
            yield return HashedValue;
        }
    }