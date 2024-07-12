using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportManagement.Application.Commands.CreateReport;
using ReportManagement.Application.DTOs;
using ReportManagement.Application.Queries.GetReportById;
using ReportManagement.Application.Queries.GetReportsByUserId;
using ReportManagement.Application.Queries.SearchReports;

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

    [HttpGet("{id}")]
    public async Task<ActionResult<ReportDto>> GetReportById(int id)
    {
        var query = new GetReportByIdQuery(id);
        var result = await _mediator.Send(query);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ReportDto>>> SearchReports([FromQuery] string? title,
        [FromQuery] string? description)
    {
        var query = new SearchReportsQuery(title, description);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<ReportDto>>> GetReportsByUserId(string userId)
    {
        var query = new GetReportsByUserIdQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}