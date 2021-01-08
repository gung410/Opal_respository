using System;
using Data.Intergration.Api.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting;

namespace cxOrganization.WebServiceAPI.Serilog
{
    public static class LoggerConfigurationRabbitMqExtension
    {
        public static void UseSerilog(this IApplicationBuilder app,
            IConfiguration Configuration,
            ILoggerFactory loggerFactory,
            Microsoft.Extensions.Logging.ILogger logger)
        {
            var rabbitMQSettings = Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

            //Using Serilog to log into Elasticsearch
            var enableSerilog =  bool.Parse(Configuration["Serilog:Enable"]);
            if (enableSerilog)
            {

                var enableRabbitmq = rabbitMQSettings.Enable;
                if (enableRabbitmq)
                {
                    logger.LogInformation("Serilog is enable, start registering queue via QueueManager API and create connection to RabbitMQ server");
                    var rabbitmqHostNames = rabbitMQSettings.HostNames;
                    var rabbitmqUsername = rabbitMQSettings.Username;
                    var rabbitmqPassword = rabbitMQSettings.Password;
                    var rabbitmqPort = rabbitMQSettings.Port;
                    if (string.IsNullOrEmpty(rabbitmqHostNames) || string.IsNullOrEmpty(rabbitmqUsername) || string.IsNullOrEmpty(rabbitmqPassword))
                    {
                        throw new ArgumentNullException("Missing Rabbitmq configure!");
                    }
                    QueueSetting queueConfiguration = new QueueSetting("ElasticsearchStoring")
                    {
                        EventRoutingKeys = new string[] { RoutingActions.AcceptAll }
                    };
                    var queueRegister = app.ApplicationServices.GetService<QueueRegister>();
                    var loggerConfiguration = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configuration)
                        .WriteTo.Console();

                    foreach (var item in queueConfiguration.EventRoutingKeys)
                    {
                        var registerResult = queueRegister.RegisterWithQueueManagerAsync(new CreateQueueConfigurationCommand
                        {
                            Exchange = queueConfiguration.EventExchange,
                            ExchangeType = queueConfiguration.ExchangeType,
                            Queue = queueConfiguration.Queue,
                            RoutingKey = item
                        });
                        if (registerResult)
                            loggerConfiguration.WriteTo.RabbitMQ(rabbitMQSettings, queueConfiguration.EventExchange, item, new LogEventFormatter());
                    }

                    Log.Logger = loggerConfiguration.CreateLogger();

                    loggerFactory.AddSerilog(dispose: true);
                }
                else
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configuration)
                        .CreateLogger();
                    loggerFactory.AddSerilog(dispose: true);
                }
            }
        }

        private static LoggerConfiguration RabbitMQ(
            this LoggerSinkConfiguration loggerConfiguration,
            RabbitMQSettings rabbitMQSettings, string exchange, string routingKey,
            ITextFormatter formatter,
            IFormatProvider formatProvider = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (rabbitMQSettings == null) throw new MissingFieldException("Rabbitmq Configuration", "rabbitMQSettings");
            if (string.IsNullOrEmpty(exchange)) throw new MissingFieldException("Rabbitmq Configuration", "exchange");
            if (string.IsNullOrEmpty(routingKey)) throw new MissingFieldException("Rabbitmq Configuration", "routingKey");
            // calls overloaded extension method
            return loggerConfiguration
                          .Sink(new RabbitMQSink(rabbitMQSettings, exchange, routingKey, formatter, formatProvider));
        }
    }
}
