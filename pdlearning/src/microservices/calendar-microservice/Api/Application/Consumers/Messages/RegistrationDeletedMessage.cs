using System;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class RegistrationDeletedMessage
    {
        public Guid UserId { get; set; }

        public Guid ClassRunId { get; set; }
    }
}
