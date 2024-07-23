using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using ReportManagement.Application.Interfaces;

namespace ReportManagement.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly string _uploadsFolderPath;

    public FileService(IConfiguration configuration)
    {
        _uploadsFolderPath = configuration["UploadsFolderPath"] ?? "uploads";
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)> GetFileStreamAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            var fullPath = Path.Combine(_uploadsFolderPath, filePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("The requested file was not found.", fullPath);

            var memory = new MemoryStream();
            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var contentType = GetMimeType(filePath);
            var fileName = Path.GetFileName(filePath);

            return (memory, contentType, fileName);
        }

    public async Task<string> SaveFileAsync(string fileName, Stream fileStream)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_uploadsFolderPath, uniqueFileName);

        Directory.CreateDirectory(_uploadsFolderPath);

        using (var fs = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fs);
        }

        return Path.GetFileName(filePath);
    }

    private string GetMimeType(string filename)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(filename, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
}
