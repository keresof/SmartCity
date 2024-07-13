using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions;
using ReportManagement.Application.Commands.CreateReport;
using ReportManagement.Domain.Repositories;
using ReportManagement.Infrastructure.Repositories;
using ReportManagement.Infrastructure.Persistence;
using UserManagement.Infrastructure.Repositories;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Application.Interfaces;
using Shared.Infrastructure;
using Shared.Infrastructure.RateLimiting;
using Shared.Infrastructure.Redis;
using Shared.Infrastructure.Persistence;
using DotNetEnv;
using System;
using System.IO;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

try
{
    var root = Directory.GetCurrentDirectory();
    var dotenv = Path.Combine(root, ".env");
    Env.Load(dotenv);
}
catch (Exception ex)
{
    Console.WriteLine($"Error loading .env file: {ex.Message}");
}

builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
if (connectionString != null)
{
    // Remove single quotes if present
    connectionString = connectionString.Trim('\'');

    // Parse the connection string
    var uri = new Uri(connectionString);
    var nBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port == -1 ? 5432 : uri.Port,
        Username = uri.UserInfo.Split(':')[0],
        Password = uri.UserInfo.Split(':')[1],
        Database = uri.LocalPath.TrimStart('/'),
        SslMode = SslMode.Require
    };

    var parsedConnectionString = nBuilder.ToString();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(parsedConnectionString));

    builder.Services.AddDbContext<UserManagementDbContext>(options =>
        options.UseNpgsql(parsedConnectionString));

    builder.Services.AddDbContext<ReportManagementDbContext>(options => 
        options.UseNpgsql(parsedConnectionString));
}
else
{
    Console.WriteLine("DefaultConnection not found in environment variables");
}

builder.Services.AddScoped<IModuleDbContext, UserManagementDbContext>();
builder.Services.AddScoped<IModuleDbContext, ReportManagementDbContext>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReportManagement API", Version = "v1" });
});

builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOTPRepository, OTPRepository>();

builder.Services.AddSingleton<RedisSlidingWindowLimiter>(sp =>
{
    var redis = RedisConnectionHelper.Connection;
    return new RedisSlidingWindowLimiter(redis);
});




builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateReportCommandHandler).Assembly));

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ReportManagementDbContext>();
        context.Database.Migrate();
        Console.WriteLine("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
    }
}

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