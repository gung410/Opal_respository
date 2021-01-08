using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Thunder.Platform.Core.Json;

namespace Conexus.Opal.Connector.RabbitMQ.Core
{
    public class OpalMessageProducer : IOpalMessageProducer
    {
        private readonly RabbitMQOptions _rabbitMQOptions;
        private readonly DefaultObjectPool<IModel> _objectPool;
        private readonly ILogger<OpalMessageProducer> _logger;

        public OpalMessageProducer(
            IOptions<RabbitMQOptions> options,
            IPooledObjectPolicy<IModel> objectPolicy,
            ILogger<OpalMessageProducer> logger)
        {
            _rabbitMQOptions = options.Value;
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy);
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task SendAsync<T>(T message, string exchangeName, string routingKey) where T : class, IMQMessage
        {
            var jsonMessage = JsonSerializer.Serialize(message, ThunderJsonSerializerOptions.SharedJsonSerializerOptions);
            return PublishMessageToQueueAsync(jsonMessage, exchangeName, routingKey);
        }

        /// <inheritdoc/>
        public Task SendAsync(string serializedMessage, string exchangeName, string routingKey)
        {
            return PublishMessageToQueueAsync(serializedMessage, exchangeName, routingKey);
        }

        /// <inheritdoc/>
        public void Send(string serializedMessage, string exchangeName, string routingKey)
        {
            PublishMessageToQueue(serializedMessage, exchangeName, routingKey);
        }

        private Task PublishMessageToQueueAsync(string message, string exchangeName, string routingKey)
        {
            PublishMessageToQueue(message, exchangeName, routingKey);

            return Task.CompletedTask;
        }

        private void PublishMessageToQueue(string message, string exchangeName, string routingKey)
        {
            var body = Encoding.UTF8.GetBytes(message);
            var channel = _objectPool.Get();

            try
            {
                channel.BasicPublish(exchangeName, routingKey, null, body);
            }
            catch (AlreadyClosedException e)
            {
                // RabbitMQ seems to be down - it will try to reconnect on its own, but this
                // particular message will NOT be delivered.
                _logger.LogError(e, "Unable to send notification");
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }
    }
}
