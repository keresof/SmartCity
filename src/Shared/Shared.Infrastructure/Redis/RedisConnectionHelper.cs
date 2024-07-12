using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

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
                string connectionString = configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connectionString);
            });
        }
    }
}