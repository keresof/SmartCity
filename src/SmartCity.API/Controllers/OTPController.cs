using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagement.Application.Commands;
using MediatR;
using Shared.Infrastructure.RateLimiting;
using System;
using SmartCity.API.Filters;

namespace SmartCity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OTPController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RedisSlidingWindowLimiter _limiter;

        public OTPController(IMediator mediator, RedisSlidingWindowLimiter limiter)
        {
            _mediator = mediator;
            _limiter = limiter;
        }

        [HttpPost("send")]
        [RateLimit("otp", 3, 60)]
        public async Task<IActionResult> SendOTP([FromBody] SendOTPCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}