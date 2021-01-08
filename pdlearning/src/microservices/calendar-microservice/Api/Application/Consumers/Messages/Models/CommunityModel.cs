using System;

namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class CommunityModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? MainCommunityId { get; set; }
    }
}
