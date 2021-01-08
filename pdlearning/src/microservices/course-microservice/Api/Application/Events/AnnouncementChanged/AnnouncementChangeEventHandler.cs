using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Microservice.Course.Domain.Entities;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events
{
    public class AnnouncementChangeEventHandler : OpalMQEventHandler<AnnouncementChangeEvent, Announcement>
    {
        public AnnouncementChangeEventHandler(IOptions<RabbitMQOptions> options, IOpalMessageProducer producer, IUserContext userContext) : base(options, producer, userContext)
        {
        }

        protected override Announcement GetMQMessagePayload(AnnouncementChangeEvent @event)
        {
            return @event.Announcement;
        }
    }
}
