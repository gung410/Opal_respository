using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Datahub.Queue.Manager.Test.MongoDbProcessor
{
    public static class ApplicationBuilderExtentions
    {
        static ConnectionFactory _factory;
        static IConnection _connection;
        static IModel _channel;
        static RabbitMQSettings _rabbitMQSettings;
        static readonly TimeSpan _delay = TimeSpan.FromSeconds(30);
        const int _retryCount = 5;
        static ILogger _logger;
        public static IApplicationBuilder UseRabbitMQListener(this IApplicationBuilder app, ILogger logger)
        {
            _logger = logger;
            var life = app.ApplicationServices.GetService<IApplicationLifetime>();
            _factory = app.ApplicationServices.GetService<ConnectionFactory>();
            _rabbitMQSettings = app.ApplicationServices.GetService<RabbitMQSettings>();

            Connect();

            life.ApplicationStarted.Register(OnStarted);
            life.ApplicationStopping.Register(OnStopping);

            return app;
        }

        private static async void OnStarted()
        {
            QueueConfiguration queueConfiguration = new QueueConfiguration()
            {
                Exchange = "event_exchange_topic",
                ExchangeType = "topic",
                Queue = "mongodb_rating_queue",
                RoutingKey = "learnapp.#",
            };
            RegisterQueue(queueConfiguration);
            await RegisterWithQueueManagerAsync(queueConfiguration);
        }

        private static async Task RegisterWithQueueManagerAsync(QueueConfiguration queueConfiguration)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(queueConfiguration);
                var result = await httpClient.PostAsync(_rabbitMQSettings.QueueManagerAPI, new StringContent(content, Encoding.UTF8, "application/json"));
                //result.EnsureSuccessStatusCode();
            }
        }

        private static void RegisterQueue(QueueConfiguration queueConfiguration)
        {
            _channel = _connection.CreateModel();

            //Processing 30 messages at the time if available
            _channel.BasicQos(0, 30, false);
            _channel.ExchangeDeclare(exchange: queueConfiguration.Exchange, type: queueConfiguration.ExchangeType, durable: true, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: queueConfiguration.Queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: queueConfiguration.Queue, exchange: queueConfiguration.Exchange, routingKey: queueConfiguration.RoutingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) => Execute(model, ea);
            _channel.BasicConsume(queue: queueConfiguration.Queue, autoAck: false, consumer: consumer);
        }

        private static void Execute(object model, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            _logger.LogWarning($"MongoDbProcessor acknowledge message: At({DateTime.UtcNow.ToString("G")}) DeliveryTag({basicDeliverEventArgs.DeliveryTag}) Redelivered({basicDeliverEventArgs.Redelivered}) ThreadId({Thread.CurrentThread.ManagedThreadId})");
            //Manual acknowledge message 
            _channel.BasicAck(deliveryTag: basicDeliverEventArgs.DeliveryTag, multiple: false);
        }

        private static void OnStopping()
        {
        }

        private static async Task ConnectionShutdownAsync(object model, ShutdownEventArgs ea)
        {
            int currentRetry = 0;
            Cleanup();
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
                    if (currentRetry > _retryCount)
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

        private static void Cleanup()
        {
            try
            {
                if (_connection != null && _connection.IsOpen)
                {
                    _connection.Close();
                    _connection = null;
                }
            }
            catch (IOException)
            {
            }
        }
    }
}
