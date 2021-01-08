using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Json;

namespace Conexus.Opal.Connector.RabbitMQ
{
    public class RabbitMQHostedService : IHostedService
    {
        private readonly ILogger<RabbitMQHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQOptions _rabbitMQOptions;
        private readonly DefaultObjectPool<IModel> _objectPool;
        private IModel _channel;

        public RabbitMQHostedService(
            IOptions<RabbitMQOptions> options,
            ILogger<RabbitMQHostedService> logger,
            IPooledObjectPolicy<IModel> objectPolicy,
            IServiceProvider serviceProvider)
        {
            _rabbitMQOptions = options.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;

            // Needs 1 object only for the hosted service.
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, 1);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _objectPool.Get();

            _channel.ExchangeDeclare(_rabbitMQOptions.DefaultCommandExchange, ExchangeType.Topic, true);
            _channel.ExchangeDeclare(_rabbitMQOptions.DefaultEventExchange, ExchangeType.Topic, true);
            _channel.ExchangeDeclare(_rabbitMQOptions.DefaultActivityExchange, ExchangeType.Topic, true);
            _channel.ExchangeDeclare(_rabbitMQOptions.DefaultAuditLogExchange, ExchangeType.Topic, true);
            _channel.ExchangeDeclare(_rabbitMQOptions.DefaultIntegrationExchange, ExchangeType.Topic, true);

            // Set exclusive to false to support multiple consumers with the same type.
            // For example: in load balancing environment, we may have 2 instances of an API.
            // RabbitMQ will automatically apply load balancing behavior to send message to 1 instance only.
            _channel.QueueDeclare(_rabbitMQOptions.QueueName, durable: true, exclusive: false);

            if (!_rabbitMQOptions.RoutingKeys.IsNullOrEmpty())
            {
                foreach (var routingKey in _rabbitMQOptions.RoutingKeys)
                {
                    _channel.QueueBind(
                        _rabbitMQOptions.QueueName,
                        _rabbitMQOptions.ExchangeName,
                        routingKey);
                }
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += OnMessageReceived;

            // autoAck: false -> the Consumer will ack manually.
            _channel.BasicConsume(queue: _rabbitMQOptions.QueueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null)
            {
                _objectPool.Return(_channel);
            }

            return Task.CompletedTask;
        }

        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs args)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("[RabbitMQHostedService] Received message with routing key: {RoutingKey}. Delivery Tag: {DeliveryTag}", args.RoutingKey, args.DeliveryTag);

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var consumers = scope.ServiceProvider.GetServices<IOpalMessageConsumer>().ToList();

                    foreach (var opalMessageConsumer in consumers)
                    {
                        if (opalMessageConsumer.CanProcess(args.RoutingKey))
                        {
                            _logger.LogInformation("Begin processing message with routing key {RoutingKey} {DeliveryTag}", args.RoutingKey, args.DeliveryTag);

                            var consumerType = opalMessageConsumer
                                .GetType()
                                .GetInterfaces()
                                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IOpalMessageConsumer<>));

                            // To ensure that the consumer implements the correct interface IOpalMessageConsumer<>.
                            // The IOpalMessageConsumer (non-generic version) is used for Interface Marker only.
                            if (consumerType == null)
                            {
                                throw new Exception("Incorrect implementation of IOpalMessageConsumer<>");
                            }

                            // Get generic type IOpalMessageConsumer<TMessage> -> TMessage
                            var argumentType = consumerType.GetGenericArguments()[0];

                            // Get type of generic OpalMQMessage<>
                            var messageType = typeof(OpalMQMessage<>);

                            // Make a generic type: OpalMQMessage<TMessage>
                            var messageGenericType = messageType.MakeGenericType(argumentType);

                            // Deserialize message into a type-safe type.
                            var data = JsonSerializer.Deserialize(args.Body.Span, messageGenericType, ThunderJsonSerializerOptions.SharedJsonSerializerOptions);

                            // Get HandleAsync method.
                            var methodInfo = consumerType.GetMethod("HandleAsync");
                            if (methodInfo == null)
                            {
                                throw new Exception($"Can not find execution method from {consumerType.FullName}");
                            }

                            AddMessageCreatedDateIntoBody(data);

                            // Invoke the method.
                            await (Task)methodInfo.Invoke(opalMessageConsumer, new[] { scope, data });

                            var messageId = data.GetType().GetProperty(nameof(OpalMQMessage<object>.Id))?.GetValue(data);
                            _logger.LogInformation("End processing message with routing key {RoutingKey} and message id {MessageId}", args.RoutingKey, messageId);
                        }
                    }

                    // Ack the message.
                    _channel.BasicAck(args.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ processing error for the routing key {Exception}", args.RoutingKey);

                // Reject the message.
                _channel.BasicReject(args.DeliveryTag, false);
            }
            finally
            {
                stopwatch.Stop();

                _logger.LogInformation(
                    "End processing message {ElapsedMilliseconds} in milliseconds.",
                    stopwatch.ElapsedMilliseconds);
            }
        }

        private void AddMessageCreatedDateIntoBody(object data)
        {
            var payloadData = data.GetType().GetProperty(nameof(OpalMQMessage<object>.Payload)).GetValue(data);
            var bodyData = payloadData.GetType().GetProperty(nameof(OpalMQMessage<object>.Payload.Body)).GetValue(payloadData);
            Type bodyType = bodyData.GetType();
            bool existMessageCreatedDate = typeof(IMQMessageHasCreatedDate).IsAssignableFrom(bodyType);
            if (existMessageCreatedDate)
            {
                var messageCreatedDateData = data.GetType().GetProperty(nameof(OpalMQMessage<object>.Created))?.GetValue(data);
                PropertyInfo messageCreatedDateInfo = bodyType.GetProperty(nameof(IMQMessageHasCreatedDate.MessageCreatedDate));
                if (messageCreatedDateInfo.CanWrite)
                {
                    messageCreatedDateInfo.SetValue(bodyData, messageCreatedDateData);
                }
            }
        }
    }
}
