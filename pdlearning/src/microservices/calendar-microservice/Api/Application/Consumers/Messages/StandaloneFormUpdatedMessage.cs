using System;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class StandaloneFormUpdatedMessage
    {
        public Guid OriginalObjectId { get; set; }

        public FormStatus Status { get; set; }
    }
}
