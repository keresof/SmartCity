using MediatR;

namespace ReportManagement.Application.Commands.UploadFile
{
    public record UploadFileCommand(string ReportId, string UserId, string FileName, Stream FileStream) : IRequest<string>;
}