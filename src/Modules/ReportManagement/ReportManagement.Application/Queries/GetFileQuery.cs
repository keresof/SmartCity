using MediatR;

namespace ReportManagement.Application.Queries
{
    public record GetFileQuery(string FileName) : IRequest<(byte[] FileContents, string ContentType)?>;
}