using Conexus.Opal.Connector.RabbitMQ.Contract;
using Microservice.Webinar.Application.RequestDtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Events
{
    public class RecordChangeEvent : BaseThunderEvent, IMQMessage
    {
        public RecordChangeEvent(SaveUploadedContentRequest model)
        {
            Model = model;
        }

        public SaveUploadedContentRequest Model { get; }

        public override string GetRoutingKey() => "microservice.events.content.create";
    }
}
