using System;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class CommunityDeletedMessage
    {
        public Guid Id { get; set; }
    }
}
