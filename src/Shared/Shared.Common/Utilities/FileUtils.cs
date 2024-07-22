using Microsoft.AspNetCore.Http;
namespace Shared.Common.Utilities;
public static class FileUtils
{
    public static async Task<byte[]> GetFileBytes(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}