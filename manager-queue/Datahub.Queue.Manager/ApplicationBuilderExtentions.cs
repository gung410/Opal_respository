using Datahub.Queue.Manager.Configurations;
using Datahub.Queue.Manager.Data;
using Datahub.Queue.Manager.Domains;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datahub.Queue.Manager
{
    public static class ApplicationBuilderExtentions
    {
        static IReadOnlyList<QueueConfiguration> _queueConfigurations { get; set; }
        static ConnectionFactory _factory;
        static IConnection _connection;
        static RabbitMQSettings _rabbitMQSettings;
        static readonly TimeSpan _delay = TimeSpan.FromSeconds(30);
        private const int MaxRetry = 5;
        static ILogger _logger;
        static IModel _channel;
        public static IApplicationBuilder UseRabbitMQ(this IApplicationBuilder app, ILogger logger)
        {
            _logger = logger;
            var life = app.ApplicationServices.GetService<IApplicationLifetime>();
            var mongoRepository = app.ApplicationServices.GetService<IMongoRepository<QueueConfiguration>>();
            _queueConfigurations = mongoRepository.List();

            _factory = app.ApplicationServices.GetService<ConnectionFactory>();
            _rabbitMQSettings = app.ApplicationServices.GetService<RabbitMQSettings>();

            Connect();

            life.ApplicationStarted.Register(OnStarted);
            life.ApplicationStopping.Register(OnStopping);

            return app;
        }

        private static void OnStarted()
        {
            _channel = _connection.CreateModel();
            //Processing 30 messages at the time if available
            _channel.BasicQos(0, 30, false);
            //Register dead-letter-exchange
            var headers = new Dictionary<string, object>();
            headers.Add("x-delayed-type", "topic");
            var deadLetterExchange = ($"{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}-datahub-{Environment.GetEnvironmentVariable("PROJECT_NAME")}-topic-dead-letter").ToLower();
            _channel.ExchangeDeclare(deadLetterExchange, "x-delayed-message", true, false, headers);
            var queueRetry = ($"{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}-datahub-{Environment.GetEnvironmentVariable("PROJECT_NAME")}-queue-retry").ToLower();
            _channel.QueueDeclare(queueRetry, false, false, false, null);
            _channel.QueueBind(queue: queueRetry, exchange: deadLetterExchange, routingKey: "#");
            _logger.LogWarning($"Register DEAD-LETTER-EXCHANGE: {deadLetterExchange} QUEUE-RETRY: {queueRetry}");

            //Handle retry for failure messages
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, basicDeliverEventArgs) => RetryProcessMessage(basicDeliverEventArgs, _channel);
            _channel.BasicConsume(queue: queueRetry, autoAck: false, consumer: consumer);

            if (_queueConfigurations != null && _queueConfigurations.Any())
            {
                foreach (var queueConfiguration in _queueConfigurations)
                {
                    RegisterQueue(queueConfiguration);
                }
            }
        }

        /// <summary>
        /// Retry 5 times if message fail
        /// </summary>
        /// <param name="basicDeliverEventArgs"></param>
        /// <param name="channel"></param>
        private static void RetryProcessMessage(BasicDeliverEventArgs basicDeliverEventArgs, IModel channel)
        {
            var body = basicDeliverEventArgs.Body;
            var message = Encoding.UTF8.GetString(body);
            var exchange = (byte[])basicDeliverEventArgs.BasicProperties.Headers["x-first-death-exchange"];
            var exchangeStr = Encoding.UTF8.GetString(exchange);
            var retryCount = 0;
            if (basicDeliverEventArgs.BasicProperties.Headers.ContainsKey("x-retry-count"))
            {
                retryCount = (int)basicDeliverEventArgs.BasicProperties.Headers["x-retry-count"];
            }
            if(retryCount >= MaxRetry)
            {
                _logger.LogError($"Cannot process message after {MaxRetry} retry: {message}");
                channel.BasicAck(deliveryTag: basicDeliverEventArgs.DeliveryTag, multiple: false);
                return;
            }
            retryCount = retryCount + 1;
            _logger.LogWarning($"Re-processing message: retry: {retryCount} message: {message}");
            IBasicProperties props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("x-retry-count", retryCount);
            //Get a random delay time to avoid running many messages at the same time.
            int delayTime = GetRandomDelayTime(retryCount);
            props.Headers.Add("x-delay", delayTime);
            props.Headers.Add("x-queue", Encoding.UTF8.GetString((byte[])basicDeliverEventArgs.BasicProperties.Headers["x-first-death-queue"]));
            channel.BasicPublish(exchangeStr, basicDeliverEventArgs.RoutingKey, props, body);
            channel.BasicAck(deliveryTag: basicDeliverEventArgs.DeliveryTag, multiple: false);
        }
        /// <summary>
        /// We have 5 times for retry, then the last one will quite immediately
        /// </summary>
        private static int[] DelayTimeArray = new int[] { 0, 300, 900, 3600, 86400, 0 }; //0, 5mins; 15mins; 1hour; 1day; 0
        private static int GetRandomDelayTime(int retryCount)
        {
            Random random = new Random();
            double randomPercentage = random.Next(50, 100);
            //Take delay time by retryCount and plus with 50 to 100 pecentage
            //So we will have random delay, to avoid running many messages at the same time.
            int delayTime = (int)(randomPercentage / 100 * DelayTimeArray[retryCount] * 1000);
            return delayTime;
        }

        private static void OnStopping()
        {
        }

        private static async Task ConnectionShutdownAsync(object model, ShutdownEventArgs ea)
        {
            int currentRetry = 0;
            CloseConnection();
            while (true)
            {
                try
                {
                    Connect();
                    break;
                }
                catch (Exception)
                {
                    currentRetry++;
                    if (currentRetry > MaxRetry)
                    {
                        throw;
                    }
                }
                await Task.Delay(_delay * currentRetry);
            }
        }

        static void Connect()
        {
            _connection = _factory.CreateConnection(_rabbitMQSettings.HostNames);
            _connection.ConnectionShutdown += async (model, ea) => await ConnectionShutdownAsync(model, ea);
        }

        private static void CloseConnection()
        {
            try
            {
                if (_channel != null && _channel.IsOpen)
                {
                    _logger.LogInformation("Close channel");
                    _channel.Close();
                    _channel = null;
                }
                if (_connection != null && _connection.IsOpen)
                {
                    _logger.LogInformation("Close Connection");
                    _connection.Close();
                    _connection = null;
                }
            }
            catch (IOException)
            {
            }
        }

        private static void RegisterQueue(QueueConfiguration queueConfiguration)
        {
            _channel.ExchangeDeclare(exchange: queueConfiguration.Exchange, type: queueConfiguration.ExchangeType, durable: true, autoDelete: false, arguments: null);
            //Set the dead letter exchange for a queue
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("x-dead-letter-exchange", ($"{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}-datahub-{Environment.GetEnvironmentVariable("PROJECT_NAME")}-topic-dead-letter"));
            _channel.QueueDeclare(queue: queueConfiguration.Queue, durable: true, exclusive: false, autoDelete: false, arguments: args);
            _channel.QueueBind(queue: queueConfiguration.Queue, exchange: queueConfiguration.Exchange, routingKey: queueConfiguration.RoutingKey);
            _logger.LogWarning($"Register Queue: {JsonConvert.SerializeObject(queueConfiguration)}");
        }
    }
}
