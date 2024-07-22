using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ReportManagement.Application.Interfaces;

namespace ReportManagement.Infrastructure.Services
{
    public class FileUploadService : IFileUploadService
    {
        private const string UploadDirectory = "uploads";

        public async Task<string> UploadFileAsync(byte[] fileContent, string fileName, string contentType, CancellationToken cancellationToken)
        {
            var uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UploadDirectory);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await File.WriteAllBytesAsync(filePath, fileContent, cancellationToken);

            return $"/{UploadDirectory}/{uniqueFileName}";
        }

        public async Task<(byte[] FileContents, string ContentType)?> GetFileAsync(string fileName, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UploadDirectory, fileName);

            if (!File.Exists(filePath))
            {
                return null;
            }

            var fileContents = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var contentType = GetContentType(fileName);

            return (fileContents, contentType);
        }

        private string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }
    }
}