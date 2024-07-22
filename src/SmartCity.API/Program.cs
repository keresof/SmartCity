using Microsoft.OpenApi.Models;
using Shared.Infrastructure.RateLimiting;
using Shared.Infrastructure.Redis;
using Shared.Common.DependencyInjection;
using DotNetEnv;
using Shared.Common.Interfaces;
using Shared.Common;
using MediatR;
using Shared.Common.Behaviors;
using SmartCity.API.Middleware;
using UserManagement.Application.Interfaces;
using UserManagement.Infrastructure.Services;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

try
{
    var root = Directory.GetCurrentDirectory();
    var dotenv = Path.Combine(root, ".env");
    Console.WriteLine($"Loading .env file from {Path.GetFullPath(dotenv)}");
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

RedisConnectionHelper.InitializeConnection(builder.Configuration);

builder.Services.AddLogging();
builder.Services.AddControllers();

var modules = ModuleDiscovery.DiscoverModules<IModuleRegistration>().ToList();
Console.WriteLine($"Discovered {modules.Count} modules");

foreach (var module in modules)
{
    Console.WriteLine($"Registering module: {module.GetType().Name}");
    module.RegisterModule(builder.Services, builder.Configuration);
}

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ValidationBehavior<,>).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Smart City API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. <br /><br /> 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      <br /><br />Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.OperationFilter<AuthorizeCheckOperationFilter>();
});

var redis = RedisConnectionHelper.Connection;

builder.Services.AddSingleton<RedisSlidingWindowLimiter>(sp =>
{
    return new RedisSlidingWindowLimiter(redis);
});

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
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
        try
        {
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

app.UseBlacklistedTokenMiddleware();

app.UseAuthentication();

app.UseAuthorization();

app.UseRefreshTokenValidationMiddleware();

app.MapControllers();

app.Run();