using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using StackExchange.Redis;
using Shared.Infrastructure.RateLimiting;

namespace Shared.Infrastructure.Tests
{
    public class RedisSlidingWindowLimiterTests
    {
        [Fact]
        public async Task TryAcquireAsync_WithinLimit_ReturnsTrue()
        {
            // Arrange
            var mockRedis = new Mock<IConnectionMultiplexer>();
            var mockDatabase = new Mock<IDatabase>();
            var mockTransaction = new Mock<ITransaction>();

            mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDatabase.Object);
            mockDatabase.Setup(d => d.CreateTransaction(It.IsAny<object>())).Returns(mockTransaction.Object);
            
            mockTransaction.Setup(t => t.SortedSetRemoveRangeByScoreAsync(It.IsAny<RedisKey>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<Exclude>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(0);
            mockTransaction.Setup(t => t.SortedSetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<double>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);
            mockTransaction.Setup(t => t.SortedSetLengthAsync(It.IsAny<RedisKey>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<Exclude>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(1);
            mockTransaction.Setup(t => t.KeyExpireAsync(It.IsAny<RedisKey>(), It.IsAny<TimeSpan>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);
            mockTransaction.Setup(t => t.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(true);

            var limiter = new RedisSlidingWindowLimiter(mockRedis.Object);

            // Act
            var result = await limiter.TryAcquireAsync("test-key", TimeSpan.FromMinutes(1), 5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task TryAcquireAsync_ExceedsLimit_ReturnsFalse()
        {
            // Arrange
            var mockRedis = new Mock<IConnectionMultiplexer>();
            var mockDatabase = new Mock<IDatabase>();
            var mockTransaction = new Mock<ITransaction>();

            mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDatabase.Object);
            mockDatabase.Setup(d => d.CreateTransaction(It.IsAny<object>())).Returns(mockTransaction.Object);
            
            mockTransaction.Setup(t => t.SortedSetRemoveRangeByScoreAsync(It.IsAny<RedisKey>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<Exclude>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(0);
            mockTransaction.Setup(t => t.SortedSetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<double>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);
            mockTransaction.Setup(t => t.SortedSetLengthAsync(It.IsAny<RedisKey>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<Exclude>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(6);
            mockTransaction.Setup(t => t.KeyExpireAsync(It.IsAny<RedisKey>(), It.IsAny<TimeSpan>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);
            mockTransaction.Setup(t => t.ExecuteAsync(It.IsAny<CommandFlags>())).ReturnsAsync(true);

            var limiter = new RedisSlidingWindowLimiter(mockRedis.Object);

            // Act
            var result = await limiter.TryAcquireAsync("test-key", TimeSpan.FromMinutes(1), 5);

            // Assert
            Assert.False(result);
        }
    }
}