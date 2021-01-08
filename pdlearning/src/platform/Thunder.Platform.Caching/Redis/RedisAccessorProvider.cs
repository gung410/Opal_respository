using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using StackExchange.Redis;

namespace Thunder.Platform.Caching.Redis
{
    public class RedisAccessorProvider : IRedisAccessorProvider, IDisposable
    {
        private readonly string _redisConnectionString;
        private readonly object _lock = new object();
        private readonly CircuitBreakerPolicy _circuitBreakerPolicy;

        private readonly ILogger _logger;
        private IDatabase _database;
        private IConnectionMultiplexer _connectionMultiplexer;
        private StringWriter _redisLog;

        public RedisAccessorProvider(IOptions<ThunderRedisCacheOptions> options, ILoggerFactory loggerFactory)
        {
            _redisConnectionString = options.Value.RedisConnectionString;
            _logger = loggerFactory.CreateLogger<RedisAccessorProvider>();

            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreaker(2, TimeSpan.FromMinutes(2));
        }

        public IDatabase GetDatabase()
        {
            lock (_lock)
            {
                EnsureConnected();
                return _database;
            }
        }

        public void CloseConnection()
        {
            lock (_lock)
            {
                try
                {
                    _connectionMultiplexer?.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "error closing redis connection");
                }

                _connectionMultiplexer = null;
                _database = null;
                _redisLog = null;
            }
        }

        public IConnectionMultiplexer GetConnection()
        {
            lock (_lock)
            {
                EnsureConnected();
                return _connectionMultiplexer;
            }
        }

        public void Dispose()
        {
            _redisLog?.Dispose();
        }

        private void InitializeRedisDatabase()
        {
            _logger.LogInformation("connection to redis instance with connection string {ConnectionString}", _redisConnectionString);
            var connectLog = new StringBuilder();
            try
            {
                _redisLog = new StringWriter(connectLog);
                _connectionMultiplexer = ConnectionMultiplexer.Connect(_redisConnectionString, _redisLog);
                _database = _connectionMultiplexer.GetDatabase();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "failed to connect to redis. Connect Log: {ConnectLog}", connectLog);
                throw;
            }
        }

        private void EnsureConnected()
        {
            lock (_lock)
            {
                void EnsureConnectedAction()
                {
                    if (_connectionMultiplexer != null && !_connectionMultiplexer.IsConnected)
                    {
                        CloseConnection();
                    }

                    if (_connectionMultiplexer == null || _database == null)
                    {
                        InitializeRedisDatabase();
                    }
                }

                _circuitBreakerPolicy.Execute(EnsureConnectedAction);
            }
        }
    }
}
