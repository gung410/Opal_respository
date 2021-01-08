using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace Conexus.Opal.Connector.RabbitMQ.Core
{
    public abstract class OpalMessageConsumer<TMessage> : IOpalMessageConsumer<TMessage>
        where TMessage : class
    {
        private bool _disposed;

        public OpalMQMessage<TMessage> OriginMessage { get; private set; }

        public async Task HandleAsync(IServiceScope serviceScope, OpalMQMessage<TMessage> message)
        {
            OriginMessage = message;
            var stopwatch = new Stopwatch();

            try
            {
                Debug.WriteLine("Begin to handle message...");
                await InternalHandleAsync(message.Payload.Body);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception, $"There is an exception when trying to send the message. Details: {exception}");
            }
            finally
            {
                OriginMessage = null;
                Debug.WriteLine($"End handling message. Total time: {stopwatch.ElapsedMilliseconds} milliseconds.");
            }
        }

        public bool CanProcess(string routingKey)
        {
            var consumerAttributes = GetType().GetCustomAttributes<OpalConsumerAttribute>().ToList();
            if (!consumerAttributes.Any())
            {
                throw new ArgumentNullException($"Please declare {nameof(OpalConsumerAttribute)} for the consumer class.");
            }

            return consumerAttributes.Any(p => p.RoutingKey == routingKey);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        protected abstract Task InternalHandleAsync(TMessage message);
    }
}
