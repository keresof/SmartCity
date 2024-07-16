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
using Shared.Common.DependencyInjection;
using DotNetEnv;
using System;
using System.IO;
using Npgsql;
using Shared.Common.Interfaces;
using Shared.Common;

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

var modules = ModuleDiscovery.DiscoverModules<IModuleRegistration>().ToList();
Console.WriteLine($"Discovered {modules.Count} modules");

foreach (var module in modules)
{
    Console.WriteLine($"Registering module: {module.GetType().Name}");
    module.RegisterModule(builder.Services, builder.Configuration);
}


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReportManagement API", Version = "v1" });
});

builder.Services.AddSingleton<RedisSlidingWindowLimiter>(sp =>
{
    RedisConnectionHelper.InitializeConnection(builder.Configuration);
    var redis = RedisConnectionHelper.Connection;
    return new RedisSlidingWindowLimiter(redis);
});

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var migrators = ModuleDiscovery.DiscoverModules<ICanMigrate>().ToList();
    Console.WriteLine($"Discovered {migrators.Count} migrators");
    foreach (var migrator in migrators)
    {
        try{
            Console.WriteLine($"Applying migrations for {migrator.GetType().Name}");
            migrator.ApplyMigrations(services);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying migrations for {migrator.GetType().Name}: {ex.Message}");
        }
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