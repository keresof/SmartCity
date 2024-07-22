using ReportManagement.Application.Interfaces;
using ReportManagement.Domain.Entities;
using ReportManagement.Domain.Repositories;
using Shared.Common.Exceptions;

namespace ReportManagement.Infrastructure.Services
{
    public class FileUploadService(IReportMediaRepository mediaRepo, IReportRepository reportRepo) : IFileUploadService
    {
        private const string UploadDirectory = "uploads";
        private readonly IReportMediaRepository _mediaRepo = mediaRepo;
        private readonly IReportRepository _reportRepo = reportRepo;

        public async Task<string> UploadFileAsync(string reportId, string userId, IEnumerable<byte> fileContent, string fileName, string contentType, CancellationToken cancellationToken)
        {
            var report = await _reportRepo.GetByIdAsync(Guid.Parse(reportId)) ?? throw new NotFoundException("Report not found");
            if(!report.UserId.Equals(Guid.Parse(userId))){
                throw new UnauthorizedAccessException("You are not authorized to upload files to this report.");
            }
            var media = new ReportMedia
            {
                ReportId = report.Id,
                MimeType = contentType,
                Filename = fileName ?? "file_" + Guid.NewGuid().ToString(),
                Media = fileContent.ToArray()
            };
            report.Medias.Append(media);
            await _mediaRepo.AddAsync(media);
            await _reportRepo.UpdateAsync(report);
            return media.Id.ToString();
        }

        public async Task<(byte[] FileContents, string ContentType)?> GetFileAsync(string mediaId, string userId, CancellationToken cancellationToken)
        {
            var media = await _mediaRepo.GetByIdAsync(Guid.Parse(mediaId));
            var report = await _reportRepo.GetByIdAsync(media.ReportId);
            if(media == null || report == null)
            {
                throw new NotFoundException("File not found.");
            }
            if (!report.UserId.Equals(Guid.Parse(userId)))
            {
                throw new UnauthorizedAccessException("You are not authorized to view this file.");
            }
            return (media.Media, media.MimeType);
        }

    }
}