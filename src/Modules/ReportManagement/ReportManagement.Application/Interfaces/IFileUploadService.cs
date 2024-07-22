using System.Threading;
using System.Threading.Tasks;

namespace ReportManagement.Application.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(string reportId, string userId, IEnumerable<byte> fileContent, string fileName, string contentType, CancellationToken cancellationToken);
        Task<(byte[] FileContents, string ContentType)?> GetFileAsync(string mediaId, string userId, CancellationToken cancellationToken);
    }
}