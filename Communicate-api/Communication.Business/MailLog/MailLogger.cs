using System;
using System.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using RabbitMQ.Client;

namespace Communication.Business.MailLog
{
    public class MailLogger : IMailLogger
    {
        private readonly IConnection _connection;
        private IModel _channel;
        private readonly MailLogConfiguration _mailLogConfiguration;
        protected readonly IServiceProvider _serviceProvider;
        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };
        public MailLogger(
            IServiceProvider serviceProvider,
            IOptions<MailLogConfiguration> option)
        {
            _mailLogConfiguration = option.Value;
            _serviceProvider = serviceProvider;
            if (_mailLogConfiguration.Enable)
            {
                _connection = _serviceProvider.GetService<IConnection>();
                _channel = _connection.CreateModel();
            }
        }

        public void WriteLog(MailLogMessage message)
        {
            if (!_mailLogConfiguration.Enable)
                return;

            var json = JsonConvert.SerializeObject(message, _jsonSettings);
            if (_channel.IsClosed)
            {
                _channel = _connection.CreateModel();
            }

            _channel.BasicPublish(exchange: _mailLogConfiguration.ExchangeName,
                                        routingKey: "maillog",
                                        basicProperties: null,
                                        body: Encoding.UTF8.GetBytes(json));
        }
    }
}
