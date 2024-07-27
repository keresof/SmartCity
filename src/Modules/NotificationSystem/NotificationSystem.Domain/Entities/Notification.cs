using Shared.Common.Abstract;
using NotificationSystem.Domain.Enums;

namespace NotificationSystem.Domain.Entities;

public abstract class Notification : AuditableEntity
{
    public string Recipient { get; protected set; }
    public string Content { get; protected set; }
    public NotificationType Type { get; protected set; }
    public NotificationStatus Status { get; protected set; }
    public DateTime? SentAt { get; protected set; }

    protected Notification(string recipient, NotificationType type)
    {
        Recipient = recipient;
        Type = type;
        Status = NotificationStatus.Pending;
        Created = DateTime.UtcNow;
    }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = NotificationStatus.Failed;
    }

    public bool IsSent()
    {
        return Status == NotificationStatus.Sent;
    }

    public bool IsFailed()
    {
        return Status == NotificationStatus.Failed;
    }

    public bool IsPending()
    {
        return Status == NotificationStatus.Pending;
    }

    public bool IsEmail()
    {
        return Type == NotificationType.Email;
    }

    public bool IsSMS()
    {
        return Type == NotificationType.SMS;
    }

    public bool IsPush()
    {
        return Type == NotificationType.Push;
    }

}
