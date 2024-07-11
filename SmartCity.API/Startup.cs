using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SmartCity.Core.Interfaces;
using SmartCity.Infrastructure;
using SmartCity.Infrastructure.Data;
using SmartCity.Infrastructure.Repositories;
using SmartCity.Modules.ReportManagement.Application.Commands;



namespace SmartCityProject
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                typeof(Startup).Assembly, 
                typeof(CreateReportCommand).Assembly));

            // Add Swagger service
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartCity API", Version = "v1" });
            });

            // Add other services...
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartCity API V1");
                c.RoutePrefix = string.Empty; 
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
