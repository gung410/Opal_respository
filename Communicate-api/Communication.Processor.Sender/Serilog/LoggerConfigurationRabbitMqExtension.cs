using Serilog;
using Serilog.Configuration;
using Serilog.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Communication.Processor.Sender.Serilog
{
    public static class LoggerConfigurationRabbitMqExtension
    {
        public static LoggerConfiguration RabbitMQ(
          this LoggerSinkConfiguration loggerConfiguration,
          RabbitMQSettings rabbitMQSettings, string exchange, string routingKey,
          ITextFormatter formatter,
          IFormatProvider formatProvider = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (rabbitMQSettings.HostNames.Length == 0 || string.IsNullOrEmpty(rabbitMQSettings.Username) || string.IsNullOrEmpty(rabbitMQSettings.Password))
            {
                throw new ArgumentNullException("Missing Rabbitmq configure!");
            }
            if (string.IsNullOrEmpty(exchange)) throw new MissingFieldException("Rabbitmq Configuration", "exchange");
            if (string.IsNullOrEmpty(routingKey)) throw new MissingFieldException("Rabbitmq Configuration", "routingKey");
            // calls overloaded extension method
            return loggerConfiguration
                          .Sink(new RabbitMQSink(rabbitMQSettings, exchange, routingKey, formatter, formatProvider));
        }
    }
}
