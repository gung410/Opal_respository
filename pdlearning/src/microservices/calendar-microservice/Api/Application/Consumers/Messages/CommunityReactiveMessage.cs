using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class CommunityReactiveMessage
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? MainCommunityId { get; set; }

        public CommunityStatus Status { get; set; }
    }
}
