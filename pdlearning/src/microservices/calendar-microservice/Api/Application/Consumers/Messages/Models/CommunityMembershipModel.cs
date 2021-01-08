using System;

namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class CommunityMembershipModel
    {
        public Guid Guid { get; set; }

        public string Email { get; set; }
    }
}
