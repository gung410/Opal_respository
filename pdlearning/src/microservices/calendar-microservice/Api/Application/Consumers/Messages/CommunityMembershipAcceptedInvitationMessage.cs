using Microservice.Calendar.Application.Consumers.Messages.Models;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class CommunityMembershipAcceptedInvitationMessage
    {
        public CommunityMembershipModel User { get; set; }

        public CommunityModel Community { get; set; }

        public CommunityMembershipRole Role { get; set; }
    }
}
