using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportManagement.Application.Commands.CreateReport;
using ReportManagement.Application.DTOs;
using ReportManagement.Application.Queries.GetReportById;

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
}