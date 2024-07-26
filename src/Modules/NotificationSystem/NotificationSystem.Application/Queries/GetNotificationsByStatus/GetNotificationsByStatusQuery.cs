using MediatR;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Application.Queries.GetNotificationsByStatus;

public class GetNotificationsByStatusQuery : IRequest<IEnumerable<Notification>>
{
    public NotificationStatus Status { get; set; }
}