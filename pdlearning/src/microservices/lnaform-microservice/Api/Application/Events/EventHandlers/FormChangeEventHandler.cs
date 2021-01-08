using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Events.EventHandlers
{
    public class FormChangeEventHandler : BaseThunderEventHandler<FormChangeEvent>
    {
        private readonly RabbitMQOptions _options;
        private readonly IOpalMessageProducer _producer;
        private readonly IUserContext _userContext;

        public FormChangeEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer, IUserContext userContext)
        {
            _options = options.Value;
            _producer = producer;
            _userContext = userContext;
        }

        protected override async Task HandleAsync(FormChangeEvent @event, CancellationToken cancellationToken)
        {
            var message = new OpalMQMessage<Domain.Entities.Form>
            {
                Type = OpalMQMessageType.Event,
                Name = @event.GetRoutingKey(),
                Routing = new OpalMQMessageRouting
                {
                    Action = @event.GetRoutingKey(),
                },
                Payload = new OpalMQMessagePayload<Domain.Entities.Form>
                {
                    Identity = new OpalMQMessageIdentity
                    {
                        SourceIp = _userContext.GetValue<string>(CommonUserContextKeys.OriginIp),
                        UserId = _userContext.GetValue<string>(CommonUserContextKeys.UserId)
                    },
                    Body = @event.Form
                }
            };

            await _producer.SendAsync(message, _options.DefaultEventExchange, @event.GetRoutingKey());
        }
    }
}
