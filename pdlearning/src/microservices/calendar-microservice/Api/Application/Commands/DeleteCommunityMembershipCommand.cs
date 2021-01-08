using System;

namespace Microservice.Calendar.Application.Commands
{
    public class DeleteCommunityMembershipCommand : BaseCalendarCommand
    {
        public Guid CommunityId { get; set; }

        public Guid UserId { get; set; }
    }
}
