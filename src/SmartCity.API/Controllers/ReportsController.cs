using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportManagement.Application.Commands.CreateReport;
using ReportManagement.Application.Commands.DeleteReport;
using ReportManagement.Application.Commands.UpdateReport;
using ReportManagement.Application.DTOs;
using ReportManagement.Application.Queries.GetReportById;
using ReportManagement.Application.Queries.GetReportsByUserId;
using ReportManagement.Application.Queries.SearchReports;
using ReportManagement.Application.Commands.UploadFile;
using Shared.Common.Exceptions;
using ReportManagement.Application.Queries;
using SmartCity.API.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace SmartCity.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IMediator mediator, ILogger<ReportsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateReport([FromBody] CreateReportCommand command)
        {
            var reportId = await _mediator.Send(command);
            return Ok(reportId);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<int>> UpdateReport(Guid id, [FromBody] UpdateReportCommand command)
        {
            if (id != command.Id) return BadRequest();

            try
            {
                var reportId = await _mediator.Send(command);
                return Ok(reportId);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> DeleteReport(Guid id, [FromBody] DeleteReportCommand command)
        {
            try
            {
                var reportId = await _mediator.Send(new DeleteReportCommand(id, command.UserId));
                return Ok(reportId);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReportDto>> GetReportById(Guid id)
        {
            try
            {
                var query = new GetReportByIdQuery(id);
                var result = await _mediator.Send(query);
                return result == null ? NotFound() : Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ReportDto>>> SearchReports([FromQuery] string? title, [FromQuery] string? location)
        {
            var query = new SearchReportsQuery(title, location);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ReportDto>>> GetReportsByUserId(Guid userId)
        {
            var query = new GetReportsByUserIdQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("{reportId}/upload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 10485760)] // 10 MB
        [RequestSizeLimit(10485760)] // 10 MB
        public async Task<ActionResult<string>> UploadFile(Guid reportId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")!.Value;
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
                return BadRequest("Not a multipart request");
            var form = await Request.ReadFormAsync();
            var file = form.Files.GetFile("file");

            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            var command = new UploadFileCommand(reportId.ToString(), userId, file.FileName, memoryStream);
            var filePath = await _mediator.Send(command);
            return Ok(filePath);
        }

        [HttpGet("media/{fileName}")]
        public async Task<IActionResult> GetFile(string fileName)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")!.Value;
            try
            {
                var query = new GetFileQuery(fileName, userId);
                var result = await _mediator.Send(query);

                return File(result.FileStream, result.ContentType, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}