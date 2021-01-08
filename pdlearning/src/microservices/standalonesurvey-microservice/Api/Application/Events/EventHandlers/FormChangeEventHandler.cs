using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.StandaloneSurvey.Application.Events.EventHandlers
{
    public class FormChangeEventHandler : BaseThunderEventHandler<SurveyChangeEvent>
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

        protected override async Task HandleAsync(SurveyChangeEvent @event, CancellationToken cancellationToken)
        {
            var message = new OpalMQMessage<Domain.Entities.StandaloneSurvey>
            {
                Type = OpalMQMessageType.Event,
                Name = @event.GetRoutingKey(),
                Routing = new OpalMQMessageRouting
                {
                    Action = @event.GetRoutingKey(),
                },
                Payload = new OpalMQMessagePayload<Domain.Entities.StandaloneSurvey>
                {
                    Identity = new OpalMQMessageIdentity
                    {
                        SourceIp = _userContext.GetValue<string>(CommonUserContextKeys.OriginIp),
                        UserId = _userContext.GetValue<string>(CommonUserContextKeys.UserId)
                    },
                    Body = @event.Survey
                }
            };

            await _producer.SendAsync(message, _options.DefaultEventExchange, @event.GetRoutingKey());
        }
    }
}
