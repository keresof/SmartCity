using MediatR;
using ReportManagement.Application.DTOs;

namespace ReportManagement.Application.Queries
{
    public record GetFileQuery(string FileName, string UserId) : IRequest<FileDownloadDto>;
}