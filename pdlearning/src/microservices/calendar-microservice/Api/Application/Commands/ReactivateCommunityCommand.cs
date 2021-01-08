using System;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Commands
{
    public class ReactivateCommunityCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public CommunityStatus Status { get; set; }

        public string Title { get; set; }

        public Guid OwnerId { get; set; }

        public Guid? ParentId { get; set; }
    }
}
