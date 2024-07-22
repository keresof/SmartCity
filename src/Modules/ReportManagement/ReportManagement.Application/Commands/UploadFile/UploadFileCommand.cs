using MediatR;

namespace ReportManagement.Application.Commands.UploadFile
{
    public record UploadFileCommand(
        byte[] FileContent,
        string FileName,
        string ContentType) : IRequest<string>;
}