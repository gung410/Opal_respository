using System;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class RegistrationChangedMessage
    {
        public Guid UserId { get; set; }

        public Guid ClassRunId { get; set; }

        public bool IsParticipant { get; set; }
    }
}
