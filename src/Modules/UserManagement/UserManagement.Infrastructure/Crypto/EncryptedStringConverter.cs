using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Common.Interfaces;

namespace UserManagement.Infrastructure.Crypto;

public class EncryptedStringConverter(IEncryptionService encryptionService) : ValueConverter<string, string>(
    v => encryptionService.Encrypt(v),
    v => encryptionService.Decrypt(v)
    )
{
}