using System;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class StandaloneFormDeletedMessage
    {
        public Guid OriginalObjectId { get; set; }
    }
}
