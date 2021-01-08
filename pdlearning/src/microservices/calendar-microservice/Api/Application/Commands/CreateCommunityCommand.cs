using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Commands
{
    public class CreateCommunityCommand : BaseCalendarCommand
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid? ParentId { get; set; }

        public Guid OwnerId { get; set; }

        public CommunityStatus Status { get; set; }
    }
}
