using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microservice.Content.Application.Events;
using Microservice.Content.Application.Models;
using Thunder.Platform.Core.Context;

namespace Microservice.Content.Infrastructure
{
    public static class OutboxQueueExtensions
    {
        public static Task QueueMessagesAsync(this IOutboxQueue queue, DigitalContentChangeType changeType, List<DigitalContentModel> digitalContentModels, IUserContext userContext)
        {
            foreach (var digitalContentModel in digitalContentModels)
            {
                var routingKey = $"microservice.events.content.{changeType}".ToLower();
                var mqMessage = BuildMQMessage(routingKey, digitalContentModel, userContext);

                var queueMessage = new QueueMessage(routingKey, mqMessage);
                queue.QueueMessageAsync(queueMessage);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// An extension method to queue the digital content changed event as a message with conventional routing key based on <see cref="DigitalContentChangeType"/>.
        /// </summary>
        /// <param name="queue">Outbox Queue.</param>
        /// <param name="changeType">Change Type.</param>
        /// <param name="digitalContentModel">Digital Content Model.</param>
        /// <param name="userContext">User Context.</param>
        /// <returns>Task.</returns>
        public static Task QueueMessageAsync(this IOutboxQueue queue, DigitalContentChangeType changeType, object digitalContentModel, IUserContext userContext)
        {
            var routingKey = $"microservice.events.content.{changeType}".ToLower();
            var mqMessage = BuildMQMessage(routingKey, digitalContentModel, userContext);

            var queueMessage = new QueueMessage(routingKey, mqMessage);
            return queue.QueueMessageAsync(queueMessage);
        }

        /// <summary>
        /// An extension method to queue the clone metadata event.
        /// </summary>
        /// <param name="queue">Outbox Queue.</param>
        /// <param name="cloneMetadataBody">Clone Info.</param>
        /// <param name="userContext">User Context.</param>
        /// <returns>Task.</returns>
        public static Task QueueMessageAsync(this IOutboxQueue queue, CloneMetadataBody cloneMetadataBody, IUserContext userContext)
        {
            var routingKey = "microservice.events.metadata.clone_resouce";
            var mqMessage = BuildMQMessage(routingKey, cloneMetadataBody, userContext);
            var queueMessage = new QueueMessage(routingKey, mqMessage);
            return queue.QueueMessageAsync(queueMessage);
        }

        private static OpalMQMessage<object> BuildMQMessage(string routingKey, object body, IUserContext userContext)
        {
            return new OpalMQMessage<object>
            {
                Type = OpalMQMessageType.Event,
                Name = routingKey,
                Routing = new OpalMQMessageRouting
                {
                    Action = routingKey
                },
                Payload = new OpalMQMessagePayload<object>
                {
                    Identity = new OpalMQMessageIdentity
                    {
                        SourceIp = userContext.GetValue<string>(CommonUserContextKeys.OriginIp),
                        UserId = userContext.GetValue<string>(CommonUserContextKeys.UserId)
                    },
                    Body = body
                }
            };
        }
    }
}
