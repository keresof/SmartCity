using MediatR;

namespace ReportManagement.Application.Commands.UploadFile
{
    public record UploadFileCommand(
        string ReportId,
        string UserId,
        IEnumerable<byte> FileContent,
        string FileName,
        string ContentType) : IRequest<string>;
}