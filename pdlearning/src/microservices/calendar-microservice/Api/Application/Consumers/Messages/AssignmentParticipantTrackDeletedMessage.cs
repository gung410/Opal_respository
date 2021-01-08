using System;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class AssignmentParticipantTrackDeletedMessage
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
    }
}
