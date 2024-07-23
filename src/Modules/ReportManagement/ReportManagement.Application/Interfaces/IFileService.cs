namespace ReportManagement.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(string fileName, Stream fileStream);
        Task<(Stream FileStream, string ContentType, string FileName)> GetFileStreamAsync(string filePath);
    }
}