using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Commands
{
    public class UpdateCommunityCommand : BaseCalendarCommand
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid? ParentId { get; set; }

        public CommunityStatus Status { get; set; }
    }
}
