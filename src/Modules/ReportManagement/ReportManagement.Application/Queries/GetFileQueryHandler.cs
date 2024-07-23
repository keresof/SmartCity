using MediatR;
using ReportManagement.Application.DTOs;
using ReportManagement.Application.Interfaces;
using ReportManagement.Domain.Repositories;
using Shared.Common.Exceptions;

namespace ReportManagement.Application.Queries;

public class GetFileQueryHandler(IFileService fileService, IMediaRepository mediaRepository) : IRequestHandler<GetFileQuery, FileDownloadDto>
{
    private readonly IFileService _fileService = fileService;
    private readonly IMediaRepository _mediaRepository = mediaRepository;

    public async Task<FileDownloadDto> Handle(GetFileQuery request, CancellationToken cancellationToken)
    {
        var media = await _mediaRepository.GetByFileNameAsync(request.FileName);

        if (media == null || !media.UserId.Equals(Guid.Parse(request.UserId))) throw new NotFoundException($"Media not found.");

        var (FileStream, ContentType, FileName) = await _fileService.GetFileStreamAsync(media.Url);

        return new  FileDownloadDto
        {
            FileStream = FileStream,
            ContentType = ContentType,
            FileName = FileName
        };
    }
}
