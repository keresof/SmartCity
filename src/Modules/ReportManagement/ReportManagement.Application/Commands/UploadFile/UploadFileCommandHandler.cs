using MediatR;
using ReportManagement.Application.Interfaces;
using ReportManagement.Domain.Entities;
using ReportManagement.Domain.Repositories;

namespace ReportManagement.Application.Commands.UploadFile;

    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, string>
    {
        private readonly IFileService _fileService;
        private readonly IReportRepository _reportRepository;
        private readonly IMediaRepository _mediaRepository;


        public UploadFileCommandHandler(IFileService fileService, IReportRepository reportRepository, IMediaRepository mediaRepository)
        {
            _fileService = fileService;
            _reportRepository = reportRepository;
            _mediaRepository = mediaRepository;
        }

        public async Task<string> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var filePath = await _fileService.SaveFileAsync(request.FileName, request.FileStream);
            var report = await _reportRepository.GetByIdAsync(Guid.Parse(request.ReportId));
            if(report == null || !report.UserId.Equals(Guid.Parse(request.UserId))){
                throw new Exception("Report not found or you are not authorized to access this report.");
            }
            report.AddMediaUrl(filePath);
            await _reportRepository.UpdateAsync(report);
            await _mediaRepository.AddAsync(new Media 
            {
                ReportId = report.Id,
                UserId = report.UserId,
                Url = filePath
            });
            return filePath;            
        }
    }
