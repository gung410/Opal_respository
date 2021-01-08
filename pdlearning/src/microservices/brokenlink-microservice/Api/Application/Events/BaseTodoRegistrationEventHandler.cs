using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;
using Thunder.Platform.Cqrs;

namespace Microservice.BrokenLink.Application.Events
{
    public abstract class BaseTodoRegistrationEventHandler<T, TPayload> : BaseThunderEventHandler<T> where T : BaseTodoRegistrationEvent where TPayload : class
    {
        private readonly IOutboxQueue _outboxQueue;
        private readonly RabbitMQOptions _rabbitMQOptions;

        protected BaseTodoRegistrationEventHandler(
            IOutboxQueue outboxQueue,
            IOptions<RabbitMQOptions> rabbitMQOptions)
        {
            _outboxQueue = outboxQueue;
            _rabbitMQOptions = rabbitMQOptions.Value;
        }

        protected abstract TPayload GetPayload(T @event);

        protected abstract List<ReceiverDto> GetAssignedTo(T @event);

        protected abstract List<ReminderByDto> GetReminderBy(T @event);

        protected abstract DateTime? GetDeadlineUTC(T @event);

        protected virtual string GetModule()
        {
            return "BrokenLink";
        }

        protected override async Task HandleAsync(T @event, CancellationToken cancellationToken)
        {
            var todoRegistrationMQMessage = new TodoRegistrationMQMessage
            {
                TaskURI = @event.TaskURI,
                Primary = new PersonInCharge
                {
                    Subject = @event.Subject,
                    AssignedTo = GetAssignedTo(@event),
                    ReminderBy = GetReminderBy(@event),
                    DeadlineUTC = GetDeadlineUTC(@event)
                },
                Module = GetModule(),
                Template = @event.Template,
                TemplateData = GetTemplateData(@event)
            };

            await _outboxQueue.QueueMessageAsync(new QueueMessage(TodoRegistrationMQMessage.RoutingKey, todoRegistrationMQMessage, _rabbitMQOptions.DefaultIntegrationExchange));
        }

        private IDictionary<string, object> GetTemplateData(T @event)
        {
            TPayload payload = GetPayload(@event);
            return payload.GetType()
                          .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                          .ToDictionary(prop => prop.Name, prop => prop.GetValue(payload, null));
        }
    }
}
