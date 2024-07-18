namespace SmartCity.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Exceptions;
using UserManagement.Application.Commands.RegisterUser;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        try{
        var result = await _mediator.Send(command);
        return Ok(result);
        }catch(ValidationException ex){
            return BadRequest(ex.Errors);
        }
    }
}