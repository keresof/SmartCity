using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagement.Application.Commands.SendOTP;
using MediatR;
using Shared.Infrastructure.RateLimiting;
using System;
using SmartCity.API.Filters;
using Shared.Common.Notifications;
using System.Linq;

namespace SmartCity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OTPController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RedisSlidingWindowLimiter _limiter;
        private readonly INotificationSender _notificationSender;

        public OTPController(IMediator mediator, RedisSlidingWindowLimiter limiter, INotificationSender notificationSender)
        {
            _mediator = mediator;
            _limiter = limiter;
            _notificationSender = notificationSender;
        }

        [HttpPost("send")]
        [RateLimit("otp", 3, 60)]
        public async Task<IActionResult> SendOTP([FromBody] SendOTPCommand command)
        {
            //var result = await _mediator.Send(command);
            var email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var otp = new Random().Next(1000, 9999);
            
            var notification = new NotificationMessage
            {
                Recipient = email,
                Type = "Email",
                Content = $"Your OTP is {otp}",
                HtmlContent = $"<h1>Your OTP is {otp}</h1>",
                Subject = "OTP"
            };
            await _notificationSender.SendEmailAsync(notification.Recipient, notification.Subject, notification.Content, notification.HtmlContent);
            return Ok();
        }
    }
}