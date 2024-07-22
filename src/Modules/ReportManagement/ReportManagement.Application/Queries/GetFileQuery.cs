using MediatR;

namespace ReportManagement.Application.Queries
{
    public record GetFileQuery(string FileId, string UserId) : IRequest<(byte[] FileContents, string ContentType)?>;
}