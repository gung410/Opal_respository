using Microservice.Course.Application.AssociatedEntities;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class ParticipantAssignmentTrackChangeEvent : BaseThunderEvent
    {
        public ParticipantAssignmentTrackChangeEvent(ParticipantAssignmentTrackAssociatedEntity participantAssignmentTrack, ParticipantAssignmentTrackChangeType changeType)
        {
            ParticipantAssignmentTrack = participantAssignmentTrack;
            ChangeType = changeType;
        }

        public ParticipantAssignmentTrackAssociatedEntity ParticipantAssignmentTrack { get; }

        public ParticipantAssignmentTrackChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.participantassignmenttrack.{ChangeType.ToString().ToLower()}";
        }
    }
}
