using System;
using System.Collections.Generic;
using System.IO;
using cxPlatform.Crosscutting.Serilog;
using RabbitMQ.Client;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Sinks.PeriodicBatching;

namespace Data.Intergration.Api.Serilog
{
    public class RabbitMQSink : PeriodicBatchingSink
    {
        private const int DefaultBatchPostingLimit = 50;
        private static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);
        private readonly ITextFormatter _formatter;
        private readonly IFormatProvider _formatProvider;
        private IModel _publishChannel;
        private IConnection _conn;
        private readonly string _exchange;
        private readonly string _routingKey;


        public RabbitMQSink(RabbitMQSettings rabbitMQSettings, string exchange, string routingKey,
            ITextFormatter formatter,
            IFormatProvider formatProvider) : base(DefaultBatchPostingLimit, DefaultPeriod)
        {
            _formatter = formatter ?? new JsonFormatter();
            _formatProvider = formatProvider;
            _exchange = exchange;
            _routingKey = routingKey;
            InitRabbitConnection(rabbitMQSettings);
        }

        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                var sw = new StringWriter();
                _formatter.Format(logEvent, sw);
                Publish(sw.ToString());
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _publishChannel.Dispose();
            _conn.Dispose();

        }

        private void InitRabbitConnection(RabbitMQSettings rabbitMQSettings)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = rabbitMQSettings.Username;
            factory.Password = rabbitMQSettings.Password;
            if (rabbitMQSettings.Port > 0)
            {
                factory.Port = rabbitMQSettings.Port;
            }
            factory.AutomaticRecoveryEnabled = true;
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
            _conn = factory.CreateConnection(rabbitMQSettings.HostNames);
            _publishChannel = _conn.CreateModel();

        }

        private void Publish(string message)
        {
            // push message to exchange
            _publishChannel.BasicPublish(_exchange, _routingKey, _publishChannel.CreateBasicProperties(), System.Text.Encoding.UTF8.GetBytes(message));
        }
    }
}
