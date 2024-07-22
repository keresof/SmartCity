using MediatR;
using ReportManagement.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ReportManagement.Application.Commands.UploadFile
{
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, string>
    {
        private readonly IFileUploadService _fileUploadService;

        public UploadFileCommandHandler(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        public async Task<string> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            return await _fileUploadService.UploadFileAsync(request.ReportId, request.UserId, request.FileContent, request.FileName, request.ContentType, cancellationToken);
        }
    }
}