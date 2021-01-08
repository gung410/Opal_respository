using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Contract;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Application.Exceptions;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Json;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events.EventHandlers
{
    public class UserActivityHappenedEventHandler : BaseThunderEventHandler<UserActivityHappenedEvent>
    {
        private const string BaseRooting = "microservice.events.learner.user_activity";

        private readonly RabbitMQOptions _options;
        private readonly TrackingEventOptions _trackingOptions;
        private readonly IOpalMessageProducer _producer;
        private readonly IUserContext _userContext;

        public UserActivityHappenedEventHandler(IOptions<RabbitMQOptions> options,  IOptions<TrackingEventOptions> trackingOptions, IOpalMessageProducer producer, IUserContext userContext)
        {
            _options = options.Value;
            _trackingOptions = trackingOptions.Value;
            _producer = producer;
            _userContext = userContext;
        }

        protected override async Task HandleAsync(UserActivityHappenedEvent @event, CancellationToken cancellationToken)
        {
            if (IsNotAllowEventThrough(@event))
            {
                throw new NotSupportedUserTrackingEventException();
            }

            var rootingKey = GetRootingKey(@event);

            var message = new OpalMQMessage<string>
            {
                Type = OpalMQMessageType.Event,
                Name = rootingKey,
                Routing = new OpalMQMessageRouting
                {
                    Action = rootingKey,
                },
                Payload = new OpalMQMessagePayload<string>
                {
                    Identity = new OpalMQMessageIdentity
                    {
                        SourceIp = _userContext.GetValue<string>(CommonUserContextKeys.OriginIp),
                        UserId = _userContext.GetValue<string>(CommonUserContextKeys.UserId)
                    },
                    Body = string.Empty
                }
            };

            string messageStr = SetBody(message, @event.UserTrackingEventRequest.UserTrackingEventAsJson);

            await _producer.SendAsync(messageStr, _options.DefaultEventExchange, rootingKey);
        }

        private bool IsNotAllowEventThrough(UserActivityHappenedEvent @event)
        {
            return _trackingOptions.AllowEvents.Contains(@event.UserTrackingEventRequest.EventName) == false;
        }

        private string SetBody(OpalMQMessage<string> message, string serializedBody)
        {
            var jsonMessage = JsonSerializer.Serialize(message, ThunderJsonSerializerOptions.SharedJsonSerializerOptions);
            var bodyPos = jsonMessage.IndexOf("\"body\":\"\"", StringComparison.Ordinal);
            var lastBodyPos = jsonMessage.LastIndexOf("\"body\":\"\"", StringComparison.Ordinal);

            if (bodyPos != lastBodyPos || bodyPos == -1)
            {
                throw new GeneralException($"{nameof(OpalMQMessage<string>)} have to have only one 'body' property");
            }

            var newJson = jsonMessage.Replace("\"body\":\"\"", $"\"body\":{serializedBody}");

            return newJson;
        }

        private string GetRootingKey(UserActivityHappenedEvent @event)
        {
            var eventName = @event.UserTrackingEventRequest.EventName;

            if (_trackingOptions.EventMapSubRouting.ContainsKey(eventName))
            {
                return $"{BaseRooting}.{_trackingOptions.EventMapSubRouting[eventName]}";
            }

            throw new UserTrackingActivityNotSupportEvent();
        }
    }
}
