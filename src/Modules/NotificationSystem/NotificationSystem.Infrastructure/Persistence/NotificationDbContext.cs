using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NotificationSystem.Domain.Entities;
using NotificationSystem.Domain.Enums;
using Shared.Infrastructure.Persistence;

namespace NotificationSystem.Infrastructure.Persistence;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options), IModuleDbContext
{
    public DbSet<EmailNotification> EmailNotifications { get; set; }
    public DbSet<SmsNotification> SmsNotifications { get; set; }
    public DbSet<PushNotification> PushNotifications { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    void IModuleDbContext.ConfigureModelBuilder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>()
            .HasDiscriminator<string>("NotificationType")
            .HasValue<EmailNotification>(NotificationType.Email.ToString())
            .HasValue<SmsNotification>(NotificationType.SMS.ToString())
            .HasValue<PushNotification>(NotificationType.Push.ToString());

        modelBuilder.Entity<Notification>()
            .Property(n => n.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Notification>()
            .Property(n => n.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Notification>()
            .HasIndex(n => n.Status);

        modelBuilder.Entity<Notification>()
            .HasIndex(n => n.Recipient);

        modelBuilder.Entity<PushNotification>()
            .Property(p => p.Data)
            .HasConversion(
                d => JsonSerializer.Serialize(d, (JsonSerializerOptions)null),
                d => JsonSerializer.Deserialize<Dictionary<string, string>>(d, (JsonSerializerOptions) null));
    }
}
