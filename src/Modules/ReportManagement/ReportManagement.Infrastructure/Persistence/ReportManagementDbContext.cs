using Microsoft.EntityFrameworkCore;
using ReportManagement.Domain.Entities;
using ReportManagement.Domain.Enums;
using Shared.Infrastructure.Persistence;

namespace ReportManagement.Infrastructure.Persistence
{
    public class ReportManagementDbContext : ModuleDbContext
    {
        public DbSet<Report> Reports { get; set; }
        public DbSet<Media> Media { get; set; }

        public ReportManagementDbContext(DbContextOptions<ReportManagementDbContext> options)
            : base(options)
        {
        }

        public override void ConfigureModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>()
                .HasIndex(r => r.Title);


            modelBuilder.Entity<Report>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Media>()
                .HasIndex(m => m.Url);
            
            modelBuilder.Entity<Media>()
                .HasIndex(m => m.ReportId);

            modelBuilder.Entity<Media>()
                .HasIndex(m => m.UserId);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>()
                .Property(r => r.Id)
                .HasColumnType("uuid")
                .ValueGeneratedOnAdd();
        }
    }
}