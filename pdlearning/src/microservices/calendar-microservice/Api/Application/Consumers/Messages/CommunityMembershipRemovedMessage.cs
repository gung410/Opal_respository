using Microservice.Calendar.Application.Consumers.Messages.Models;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class CommunityMembershipRemovedMessage
    {
        public CommunityMembershipModel User { get; set; }

        public CommunityModel Community { get; set; }
    }
}
