using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportManagement.Application.Commands.CreateReport;
using ReportManagement.Application.Commands.DeleteReport;
using ReportManagement.Application.Commands.UpdateReport;
using ReportManagement.Application.DTOs;
using ReportManagement.Application.Queries.GetReportById;
using ReportManagement.Application.Queries.GetReportsByUserId;
using ReportManagement.Application.Queries.SearchReports;
using Shared.Common.Exceptions;

namespace SmartCity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateReport([FromBody] CreateReportCommand command)
    {
        var reportId = await _mediator.Send(command);
        return Ok(reportId);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<int>> UpdateReport(int id, [FromBody] UpdateReportCommand command)
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
    public async Task<ActionResult<int>> DeleteReport(int id, [FromBody] DeleteReportCommand command)
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
    public async Task<ActionResult<ReportDto>> GetReportById(int id)
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
    public async Task<ActionResult<List<ReportDto>>> SearchReports([FromQuery] string? title,
        [FromQuery] string? location)
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
}