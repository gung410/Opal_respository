using System;

namespace Microservice.Calendar.Application.Commands
{
    public class DeleteCommunityCommand : BaseCalendarCommand
    {
        public Guid CommunityId { get; set; }
    }
}
