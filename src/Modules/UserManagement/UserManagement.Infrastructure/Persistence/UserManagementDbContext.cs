using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Persistence
{
    public class UserManagementDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<OTP> OTPs { get; set; }

        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
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
        }
    }
}