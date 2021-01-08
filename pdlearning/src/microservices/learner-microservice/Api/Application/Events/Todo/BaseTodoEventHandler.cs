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

namespace Microservice.Learner.Application.Events.Todo
{
    public abstract class BaseTodoEventHandler<T, TPayload> : BaseThunderEventHandler<T> where T : BaseTodoEvent<TPayload> where TPayload : BaseTodoEventPayload
    {
        private readonly RabbitMQOptions _options;
        private readonly IOutboxQueue _outboxQueue;

        protected BaseTodoEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue outboxQueue)
        {
            _options = options.Value;
            _outboxQueue = outboxQueue;
        }

        protected abstract TPayload GetPayload(T @event);

        protected abstract List<ReceiverDto> GetPrimary(T @event);

        protected virtual List<ReminderByDto> GetReminderByPrimary(T @event)
        {
            return null;
        }

        protected virtual string GetMessagePrimary(T @event)
        {
            return string.Empty;
        }

        protected virtual string GetPlainTextPrimary(T @event)
        {
            return string.Empty;
        }

        protected virtual string GetModule()
        {
            return "LEARNER";
        }

        protected override async Task HandleAsync(T @event, CancellationToken cancellationToken)
        {
            var todoRegistrationMQMessage = new TodoRegistrationMQMessage()
            {
                TaskURI = @event.TaskURI,
                CreatedBy = @event.CreatedBy.ToString(),
                Primary = new PersonInCharge()
                {
                    Subject = @event.Subject,
                    AssignedTo = GetPrimary(@event),
                    ReminderBy = GetReminderByPrimary(@event),
                    Message = GetMessagePrimary(@event),
                    PlainText = GetPlainTextPrimary(@event)
                },
                Module = GetModule(),
                Template = @event.Template,
                TemplateData = GetTemplateData(@event)
            };
            await _outboxQueue.QueueMessageAsync(new QueueMessage(TodoRegistrationMQMessage.RoutingKey, todoRegistrationMQMessage, _options.DefaultIntegrationExchange));
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
