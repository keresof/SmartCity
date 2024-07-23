using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportManagement.Application.Commands.CreateReport;
using ReportManagement.Domain.Repositories;
using ReportManagement.Infrastructure.Persistence;
using ReportManagement.Infrastructure.Repositories;
using ReportManagement.Application.Interfaces;
using ReportManagement.Infrastructure.Services;
using Shared.Common.Interfaces;
using Shared.Common.Utilities;

namespace ReportManagement.Infrastructure.DependencyInjection;

public class ReportManagementModuleRegistration : IModuleRegistration
{
    public void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = ConnectionStringParser.ConvertToNpgsqlFormat(configuration["DefaultConnection"]!);
        services.AddDbContext<ReportManagementDbContext>(
                options => options.UseNpgsql(connectionString)
            )
            .AddScoped<IReportRepository, ReportRepository>()
            .AddScoped<IMediaRepository, MediaRepository>()
            .AddScoped<IFileService, FileService>()
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateReportCommand).Assembly);
            });
    }
}