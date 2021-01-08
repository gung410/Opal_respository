using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace Conexus.Opal.Connector.RabbitMQ.Core
{
    public interface IOpalMessageConsumer
    {
        bool CanProcess(string routingKey);
    }

    public interface IOpalMessageConsumer<TMessage> : IOpalMessageConsumer, IDisposable where TMessage : class
    {
        /// <summary>
        /// Handle Opal MQ Message. Please DO NOT CHANGE this interface due to usage of RabbitMQHostedService.
        /// </summary>
        /// <param name="serviceScope">The current scope.</param>
        /// <param name="message">The MQ message coming from Rabbit MQ.</param>
        /// <returns>A waitable task.</returns>
        Task HandleAsync(IServiceScope serviceScope, OpalMQMessage<TMessage> message);
    }
}
