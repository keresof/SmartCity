using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using Shared.Infrastructure.Persistence;
using UserManagement.Infrastructure.Crypto;
using Shared.Common.Interfaces;

namespace UserManagement.Infrastructure.Persistence;

public class UserManagementDbContext : ModuleDbContext
{

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
        : base(options)
    {
    }

    public override void ConfigureModelBuilder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);

            builder.OwnsOne(u => u.PasswordHash, passwordBuilder =>
            {
                passwordBuilder.Property(p => p.Hash).HasColumnName("PasswordHash");
            });

            builder.OwnsOne(u => u.Email, emailBuilder =>
            {
                emailBuilder.Property(e => e.EncryptedValue).HasColumnName("Email");
            });

            builder.OwnsOne(u => u.FirstName, firstNameBuilder =>
            {
                firstNameBuilder.Property(f => f.EncryptedValue).HasColumnName("FirstName");
            });

            builder.OwnsOne(u => u.LastName, lastNameBuilder =>
            {
                lastNameBuilder.Property(l => l.EncryptedValue).HasColumnName("LastName");
            });

            builder.OwnsOne(u => u.RefreshToken, refreshTokenBuilder =>
            {
                refreshTokenBuilder.Property(r => r.EncryptedValue).HasColumnName("RefreshToken");
            });
            builder.OwnsOne(u => u.ResetPasswordToken, resetPasswordTokenBuilder =>
            {
                resetPasswordTokenBuilder.Property(r => r.EncryptedValue).HasColumnName("ResetPasswordToken");
            });
            builder.OwnsOne(u => u.PhoneNumber, phoneNumberBuilder =>
            {
                phoneNumberBuilder.Property(p => p.EncryptedValue).HasColumnName("PhoneNumber");
            });
            builder.Property(u => u.EmailHash).HasColumnName("EmailHash");
            builder.Property(u => u.FirstNameHash).HasColumnName("FirstNameHash");
            builder.Property(u => u.LastNameHash).HasColumnName("LastNameHash");
            builder.Property(u => u.PhoneNumberHash).HasColumnName("PhoneNumberHash");
            builder.Property(u => u.RefreshTokenHash).HasColumnName("RefreshTokenHash");
            builder.Property(u => u.ResetPasswordTokenHash).HasColumnName("ResetPasswordTokenHash");

            builder.HasIndex(u => u.EmailHash).IsUnique();
            builder.HasIndex(u => u.RefreshTokenHash);

        });

        modelBuilder.Entity<Role>(builder =>
        {
            builder.HasKey(r => r.Id);
            builder.HasIndex(r => r.Name).IsUnique();
        });

        modelBuilder.Entity<Permission>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Name).IsUnique();
        });

    }
}
