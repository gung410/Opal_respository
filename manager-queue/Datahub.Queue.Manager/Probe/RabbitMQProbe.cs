using Datahub.Queue.Manager.Configurations;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;

namespace Datahub.Queue.Manager.Probe
{
    public class RabbitMQProbe : IHealthCheckProbe
    {
        private readonly IOptions<RabbitMQSettings> _rabbitMQSetting;
        private readonly ConnectionFactory _factory;
        public RabbitMQProbe(IOptions<RabbitMQSettings> rabbitMQSetting,
            ConnectionFactory factory)
        {
            _rabbitMQSetting = rabbitMQSetting;
            _factory = factory;
        }
        public HealthStatus ExecuteProbe(params string[] probeParameters)
        {
            try
            {
                var connection = _factory.CreateConnection(_rabbitMQSetting.Value.HostNames);
                connection.Close();
                return new HealthStatus { IsAlive = true };
            }
            catch (Exception e)
            {
                return new HealthStatus { IsAlive = false, ErrorMessage = e.Message };
            }
        }
    }
}
