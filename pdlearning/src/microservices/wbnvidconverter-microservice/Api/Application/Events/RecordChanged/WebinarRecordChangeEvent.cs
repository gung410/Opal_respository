using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.WebinarVideoConverter.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.WebinarVideoConverter.Application.Events
{
    public class WebinarRecordChangeEvent : BaseThunderEvent, IMQMessage
    {
        public WebinarRecordChangeEvent(WebinarRecordChangeRequest request)
        {
            Model = request;
        }

        public WebinarRecordChangeRequest Model { get; }

        public override string GetRoutingKey() => $"microservice.events.webinar-record-converter.converting-tracking.{Model.Status}".ToLower();
    }
}
