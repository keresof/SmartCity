using Microsoft.EntityFrameworkCore;
using ReportManagement.Domain.Entities;
using ReportManagement.Domain.Enums;
using Shared.Infrastructure.Persistence;

namespace ReportManagement.Infrastructure.Persistence
{
    public class ReportManagementDbContext : ModuleDbContext
    {
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportMedia> ReportMedias { get; set; }

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

            modelBuilder.Entity<ReportMedia>()
                .Property(rm => rm.ReportId)
                .HasColumnType("uuid");

            modelBuilder.Entity<ReportMedia>()
                .Property(rm => rm.Id)
                .HasColumnType("uuid")
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ReportMedia>()
                .HasIndex(rm => rm.ReportId);
            
            modelBuilder.Entity<ReportMedia>()
                .HasIndex(rm => rm.Id);

                
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