using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;
using Shared.Infrastructure.Persistence;

namespace UserManagement.Infrastructure.Persistence
{
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
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.AuthenticationMethods)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(e => Enum.Parse<AuthenticationMethod>(e))
                         .ToList());

            modelBuilder.Entity<User>()
                .OwnsOne(u =>u.PasswordHash, ph => 
                {
                    ph.Property(p => p.Hash).HasColumnName("PasswordHash").IsRequired(false);
                });
                         
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();
            modelBuilder.Entity<Permission>()
                .HasIndex(p => p.Name)
                .IsUnique();
        }
    }
}