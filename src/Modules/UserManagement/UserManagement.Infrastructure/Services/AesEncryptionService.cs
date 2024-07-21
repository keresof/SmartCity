using System.Security.Cryptography;
using System.Text;
using Shared.Common.Interfaces;


namespace UserManagement.Infrastructure.Services;

public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public AesEncryptionService(string key)
    {
        _key = ConvertHexStringToByteArray(key);
    }

    private static byte[] ConvertHexStringToByteArray(string hexString)
    {
        if (hexString.Length % 2 != 0)
        {
            throw new ArgumentException("The hex string must have an even number of characters.");
        }

        byte[] result = new byte[hexString.Length / 2];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
        }
        return result;
    }

    public string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV(); // Generate a new IV for each encryption

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using MemoryStream memoryStream = new();
        memoryStream.Write(aes.IV, 0, aes.IV.Length); // Write IV to the output stream

        using (CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write))
        using (StreamWriter streamWriter = new(cryptoStream))
        {
            streamWriter.Write(plainText);
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using Aes aes = Aes.Create();
        aes.Key = _key;

        byte[] iv = new byte[aes.IV.Length];
        Array.Copy(cipherBytes, 0, iv, 0, iv.Length); // Extract IV from the beginning of cipherBytes
        aes.IV = iv;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using MemoryStream memoryStream = new(cipherBytes, iv.Length, cipherBytes.Length - iv.Length);
        using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
        using StreamReader streamReader = new(cryptoStream);

        return streamReader.ReadToEnd();
    }
}