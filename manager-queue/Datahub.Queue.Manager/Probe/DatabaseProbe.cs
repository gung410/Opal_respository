using Datahub.Queue.Manager.Data.MongoDb;
using Microsoft.Extensions.Options;
using System;

namespace Datahub.Queue.Manager.Probe
{
    public class DatabaseProbe : IHealthCheckProbe
    {
        private readonly MongoDbContext _context;
        private readonly IOptions<MongoDbSettings> _settings;
        public DatabaseProbe(IOptions<MongoDbSettings> settings)
        {
            _context = new MongoDbContext(settings);
            _settings = settings;
        }

        public HealthStatus ExecuteProbe(params string[] probeParameters)
        {
            try
            {
                if (_context.IsClusterConnceted && _context.IsServerConnceted)
                {
                    return new HealthStatus { IsAlive = true };
                }
                else
                {
                    return new HealthStatus { IsAlive = false, ErrorMessage = $"Cannot connect to MongoDb: {_settings.Value.ConnectionString} DatabaseName: {_settings.Value.DatabaseName}" };
                }
            }
            catch (Exception e)
            {
                return new HealthStatus { IsAlive = false, ErrorMessage = e.Message };
            }
        }
    }
}
