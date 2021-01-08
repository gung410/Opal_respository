using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Commands
{
    public class CreateCommunityMembershipCommand : BaseCalendarCommand
    {
        public Guid CommunityId { get; set; }

        public Guid UserId { get; set; }

        public CommunityMembershipRole Role { get; set; }
    }
}
