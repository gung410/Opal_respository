using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Commands
{
    public class ChangeOwnerCommunityCommand : BaseCalendarCommand
    {
        public Guid CommunityId { get; set; }

        public Guid NewOwnerId { get; set; }
    }
}
