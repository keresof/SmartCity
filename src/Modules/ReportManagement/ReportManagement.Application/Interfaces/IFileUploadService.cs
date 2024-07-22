using System.Threading;
using System.Threading.Tasks;

namespace ReportManagement.Application.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(byte[] fileContent, string fileName, string contentType, CancellationToken cancellationToken);
        Task<(byte[] FileContents, string ContentType)?> GetFileAsync(string fileName, CancellationToken cancellationToken);
    }
}