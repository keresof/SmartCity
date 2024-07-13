// Shared.Infrastructure.ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using ReportManagement.Domain.Entities;

namespace Shared.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Report>()
                .HasIndex(r => r.UserId);  // This creates an index on UserId for faster lookups
        }
    }
}