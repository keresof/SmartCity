using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Domain.Interfaces;

public interface INotificationRepository
{
    Task SaveAsync(Notification notification);
    Task<Notification?> GetByIdAsync(Guid id);
    Task<IEnumerable<Notification>> GetNotificationsByStatusAsync(NotificationStatus status);
}