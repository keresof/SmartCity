using MediatR;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;


namespace UserManagement.Application.Commands
{
    public class SendOTPCommand : IRequest<SendOTPResult>
    {
        public string UserId { get; set; }
        public OTPPurpose Purpose { get; set; }
        public OTPDeliveryMethod DeliveryMethod { get; set; }
    }

    public class SendOTPResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}