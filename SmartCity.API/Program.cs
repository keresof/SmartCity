
using SmartCity.Core.Interfaces;
using SmartCity.Infrastructure;
using SmartCity.Infrastructure.Data;
using SmartCity.Infrastructure.Repositories;
using SmartCity.Modules.ReportManagement.Application.Commands;
using SmartCityProject;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
