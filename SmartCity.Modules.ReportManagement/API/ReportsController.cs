//TODO: MVC 
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Modules.ReportManagement.Application.Commands;


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
    public async Task<ActionResult<int>> Create(CreateReportCommand command)
    {
        var reportId = await _mediator.Send<int>(command);
        return Ok(reportId);
    }
}