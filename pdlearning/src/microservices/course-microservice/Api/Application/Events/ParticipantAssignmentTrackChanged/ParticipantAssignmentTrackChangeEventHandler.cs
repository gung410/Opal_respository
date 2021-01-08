using Conexus.Opal.Connector.RabbitMQ;
using Conexus.Opal.Connector.RabbitMQ.Extensions;
using Conexus.Opal.OutboxPattern;
using Microservice.Course.Application.AssociatedEntities;
using Microservice.Course.Application.Models;
using Microsoft.Extensions.Options;
using Thunder.Platform.Core.Context;

namespace Microservice.Course.Application.Events
{
    public class ParticipantAssignmentTrackChangeEventHandler : OutboxOpalMqEventHandler<ParticipantAssignmentTrackChangeEvent, ParticipantAssignmentTrackAssociatedEntity>
    {
        public ParticipantAssignmentTrackChangeEventHandler(IOptions<RabbitMQOptions> options, IOutboxQueue queue, IUserContext userContext) : base(options, userContext, queue)
        {
        }

        protected override ParticipantAssignmentTrackAssociatedEntity GetMQMessagePayload(ParticipantAssignmentTrackChangeEvent @event)
        {
            return @event.ParticipantAssignmentTrack;
        }
    }
}
