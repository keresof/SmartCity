using MediatR;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Interfaces;

namespace NotificationSystem.Application.Queries.GetNotificationsByStatus;

public class GetNotificationsByStatusQueryHandler : IRequestHandler<GetNotificationsByStatusQuery, IEnumerable<Notification>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetNotificationsByStatusQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<Notification>> Handle(GetNotificationsByStatusQuery request, CancellationToken cancellationToken)
    {
        return await _notificationRepository.GetNotificationsByStatusAsync(request.Status);
    }
}