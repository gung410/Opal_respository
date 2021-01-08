using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Learner.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Events
{
    public class UserActivityHappenedEvent : BaseThunderEvent, IMQMessage
    {
        public UserActivityHappenedEvent(UserTrackingEventRequest userTrackingEventRequest)
        {
            UserTrackingEventRequest = userTrackingEventRequest;
        }

        public UserTrackingEventRequest UserTrackingEventRequest { get; }

        public override string GetRoutingKey()
        {
            return "not_using_this_routing";
        }
    }
}
