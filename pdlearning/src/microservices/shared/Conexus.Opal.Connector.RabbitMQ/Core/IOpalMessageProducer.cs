using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;

namespace Conexus.Opal.Connector.RabbitMQ.Core
{
    public interface IOpalMessageProducer
    {
        /// <summary>
        /// Send a message to rabbitmq mq in a type-safety way.
        /// </summary>
        /// <typeparam name="T">The type of a message.</typeparam>
        /// <param name="message">The message to be sent.</param>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <returns>A completed task.</returns>
        Task SendAsync<T>(T message, string exchangeName, string routingKey) where T : class, IMQMessage;

        /// <summary>
        /// Send a serialized message (as JSON format) to the rabbit mq.
        /// </summary>
        /// <param name="serializedMessage">The serialized message to be sent.</param>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <returns>A completed task.</returns>
        Task SendAsync(string serializedMessage, string exchangeName, string routingKey);

        /// <summary>
        /// Send a serialized message (as JSON format) to the rabbit mq with the SYNC version.
        /// Avoid to use this SYNC version, please use ASYNC version instead.
        /// </summary>
        /// <param name="serializedMessage">The serialized message to be sent.</param>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="routingKey">The routing key.</param>
        void Send(string serializedMessage, string exchangeName, string routingKey);
    }
}
