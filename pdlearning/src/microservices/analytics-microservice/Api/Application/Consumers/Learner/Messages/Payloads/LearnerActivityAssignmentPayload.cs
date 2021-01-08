using System;

namespace Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads
{
    public class LearnerActivityAssignmentPayload
    {
        public Guid PlayingSessionId { get; set; }

        public Guid AssignmentId { get; set; }

        public Guid ParticipantAssignmentTrackId { get; set; }
    }
}
