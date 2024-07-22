using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using System;

namespace Shared.Infrastructure.Redis
{
    public class RedisConnectionHelper
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        public static void InitializeConnection(IConfiguration configuration)
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string connectionString = configuration["Redis"];
                Console.WriteLine($"Attempting to connect to Redis with connection string: {connectionString}");

                var options = ConfigurationOptions.Parse(connectionString);
                options.AbortOnConnectFail = false;  // Don't throw exception on connection failure
                options.ConnectRetry = 5;
                options.ConnectTimeout = 10000;  // 10 seconds

                try
                {
                    var muxer = ConnectionMultiplexer.Connect(options);
                    Console.WriteLine("Successfully connected to Redis");
                    return muxer;
                }
                catch (RedisConnectionException ex)
                {
                    Console.WriteLine($"Failed to connect to Redis: {ex.Message}");
                    throw;
                }
            });
        }
    }
}