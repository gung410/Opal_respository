using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.OutboxPattern;
using Microsoft.Extensions.Options;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Events
{
    public abstract class BaseContentTodoRegistrationEventHandler<T, TPayload> : BaseThunderEventHandler<T> where T : BaseContentTodoRegistrationEvent where TPayload : class
    {
        private readonly RabbitMQOptions _options;
        private readonly IOpalMessageProducer _producer;
        private readonly RabbitMQOptions _rabbitMQOptions;

        protected BaseContentTodoRegistrationEventHandler(
            IOptions<RabbitMQOptions> options,
            IOpalMessageProducer producer)
        {
            _options = options.Value;
            _producer = producer;
            _rabbitMQOptions = options.Value;
        }

        protected abstract TPayload GetPayload(T @event);

        protected abstract List<ReceiverDto> GetAssignedTo(T @event);

        protected abstract List<ReminderByDto> GetReminderBy(T @event);

        protected abstract DateTime? GetDeadlineUTC(T @event);

        protected virtual string GetModule()
        {
            return "CCPM";
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

            await _producer.SendAsync(todoRegistrationMQMessage, _options.DefaultIntegrationExchange, TodoRegistrationMQMessage.RoutingKey);
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
