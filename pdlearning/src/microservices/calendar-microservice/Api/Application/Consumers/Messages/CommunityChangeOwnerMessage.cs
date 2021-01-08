using System;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class CommunityChangeOwnerMessage
    {
        public Guid Id { get; set; }

        public Guid CreatedBy { get; set; }
    }
}
