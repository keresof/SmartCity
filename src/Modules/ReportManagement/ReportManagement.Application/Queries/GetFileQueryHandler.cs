using MediatR;
using ReportManagement.Application.Interfaces;

namespace ReportManagement.Application.Queries
{
    public class GetFileQueryHandler : IRequestHandler<GetFileQuery, (byte[] FileContents, string ContentType)?>
    {
        private readonly IFileUploadService _fileUploadService;

        public GetFileQueryHandler(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        public async Task<(byte[] FileContents, string ContentType)?> Handle(GetFileQuery request, CancellationToken cancellationToken)
        {
            return await _fileUploadService.GetFileAsync(request.FileName, cancellationToken);
        }
    }
}