using System;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class SessionDeletedMessage
    {
        public Guid Id { get; set; }

        public Guid ClassRunId { get; set; }
    }
}
