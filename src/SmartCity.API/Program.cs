using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ReportManagement.Application.Commands.CreateReport;
using ReportManagement.Domain.Repositories;
using ReportManagement.Infrastructure.Repositories;
using UserManagement.Infrastructure.Repositories;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Application.Interfaces;
using Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReportManagement API", Version = "v1" });
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ReportManagementDb"));
builder.Services.AddDbContext<UserManagementDbContext>(options =>
    options.UseInMemoryDatabase("UserManagementDb"));


builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOTPRepository, OTPRepository>();




builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateReportCommandHandler).Assembly));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReportManagement API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();