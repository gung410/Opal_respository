using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.StandaloneSurvey.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Events.EventHandlers
{
    public class FormSubmitEventHandler : BaseThunderEventHandler<FormSubmitEvent>
    {
        private readonly RabbitMQOptions _options;
        private readonly IOpalMessageProducer _producer;
        private readonly IUserContext _userContext;

        public FormSubmitEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer, IUserContext userContext)
        {
            _options = options.Value;
            _producer = producer;
            _userContext = userContext;
        }

        protected override async Task HandleAsync(FormSubmitEvent @event, CancellationToken cancellationToken)
        {
            var message = new OpalMQMessage<FormSubmitEventModel>
            {
                Type = OpalMQMessageType.Event,
                Name = @event.GetRoutingKey(),
                Routing = new OpalMQMessageRouting
                {
                    Action = @event.GetRoutingKey(),
                },
                Payload = new OpalMQMessagePayload<FormSubmitEventModel>
                {
                    Identity = new OpalMQMessageIdentity
                    {
                        SourceIp = _userContext.GetValue<string>(CommonUserContextKeys.OriginIp),
                        UserId = _userContext.GetValue<string>(CommonUserContextKeys.UserId)
                    },
                    Body = @event.FormSubmitModel
                }
            };

            await _producer.SendAsync(message, _options.DefaultEventExchange, @event.GetRoutingKey());
        }
    }
}
