using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.IO;
using DotNetEnv;
using Npgsql;

namespace ReportManagement.Infrastructure.Persistence
{
    public class ReportManagementDbContextFactory : IDesignTimeDbContextFactory<ReportManagementDbContext>
    {
        public ReportManagementDbContext CreateDbContext(string[] args)
        {
            // Load .env file
            DotNetEnv.Env.Load();

            // Get connection string from environment variable
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
                if (string.IsNullOrEmpty(parsedConnectionString))
                {
                    throw new InvalidOperationException("Hmmm.. The DefaultConnection string is not set in the .env file.");
                }

                // Create DbContextOptions
                var optionsBuilder = new DbContextOptionsBuilder<ReportManagementDbContext>();
                optionsBuilder.UseNpgsql(parsedConnectionString);

                return new ReportManagementDbContext(optionsBuilder.Options);

            }
            else
            {
                Console.WriteLine("DefaultConnection not found in environment variables");
                throw new InvalidOperationException("The DefaultConnection string is not set in the .env file.");
            }


        }
    }
}