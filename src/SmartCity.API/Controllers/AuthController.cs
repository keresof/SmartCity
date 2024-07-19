namespace SmartCity.API.Controllers;

using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Exceptions;
using UserManagement.Application.Commands.BuildOAuthChallengeUrl;
using UserManagement.Application.Commands.HandleOAuthCallback;
using UserManagement.Application.Commands.RegisterUser;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        _logger.LogInformation("Registering user with email: {Email}", command.Email);
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Errors);
        }
    }

    [HttpGet("o/{provider}")]
    public async Task<IActionResult> ExternalLogin(string provider)
    {
        try
        {
            _logger.LogInformation("External login request for provider: {Provider}", provider);
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest("Provider is required.");
            }

            var supportedProviders = new[] { "google", "microsoft" };
            if (!supportedProviders.Contains(provider.ToLower()))
            {
                return BadRequest(string.Format("Provider '{0}' is not supported.", provider));
            }

            var redirectUri = Url.Action(nameof(ExternalLoginCallback), "Auth", new { provider }, Request.Scheme);
            var command = new BuildOAuthChallengeUrlCommand
            {
                ProviderName = provider,
                RedirectUri = redirectUri
            };

            var challengeUrl = await _mediator.Send(command);
            _logger.LogInformation("Redirecting to challenge URL: {ChallengeUrl}", challengeUrl);
            Response.Headers["Location"] = challengeUrl;
            Response.StatusCode = 200;
            return new EmptyResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ExternalLogin for provider: {Provider}", provider);
            return StatusCode(500, "An unexpected error occurred. Please check server logs.");
        }
    }

    [HttpGet("o/{provider}/cb")]
    public async Task<IActionResult> ExternalLoginCallback(string provider, [FromQuery] string code, [FromQuery] string state)
    {
        try
        {
            var redirectUri = Url.Action(nameof(ExternalLoginCallback), "Auth", new { provider }, Request.Scheme);
            var command = new HandleOAuthCallbackCommand
            {
                ProviderName = provider,
                Code = code,
                State = state,
                RedirectUri = redirectUri
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ExternalLoginCallback for provider: {Provider}", provider);
            return StatusCode(500, "An unexpected error occurred. Please check server logs.");
        }

    }
}