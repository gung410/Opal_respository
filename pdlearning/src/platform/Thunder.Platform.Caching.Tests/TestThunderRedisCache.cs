using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thunder.Platform.Caching.Redis;
using Thunder.Platform.Testing;
using Xunit;

namespace Thunder.Platform.Caching.Tests
{
    public class TestThunderRedisCache : BaseThunderTest
    {
        [Fact]
        public void ConnectToRedisShouldBeOk()
        {
            var options = Provider.GetService<IOptions<ThunderRedisCacheOptions>>();
            var loggerFactory = Provider.GetService<ILoggerFactory>();

            using (var redisAccessorProvider = new RedisAccessorProvider(options, loggerFactory))
            {
                var cacheProvider = new CacheDataProvider(new ICacheRepository[]
                {
                    new RedisCacheRepository(
                        "AppSettings",
                        redisAccessorProvider,
                        loggerFactory)
                });

                var cacheKey = new CacheKey("AppSettings", "Hello", false);
                cacheProvider.AddCachedData(cacheKey, true, TimeSpan.MaxValue);

                var cacheValue = cacheProvider.GetCachedData<bool>(cacheKey);

                Assert.True(cacheValue);
            }
        }

        protected override IEnumerable<KeyValuePair<string, string>> SetupInMemoryConfiguration()
        {
            return new[]
            {
                new KeyValuePair<string, string>(
                    "ThunderRedisCacheOptions:RedisConnectionString",
                    "thunder.redis.cache.windows.net:6380,password=qeyXkvM3gUGBme177Ii5Y8HRzBVPV9b2ltTsVzBwvHQ=,ssl=True,abortConnect=False")
            };
        }

        protected override IServiceCollection SetupAppServices(IServiceCollection services)
        {
            services.AddOptions<ThunderRedisCacheOptions>().Bind(Configuration.GetSection("ThunderRedisCacheOptions"));
            return base.SetupAppServices(services);
        }
    }
}
