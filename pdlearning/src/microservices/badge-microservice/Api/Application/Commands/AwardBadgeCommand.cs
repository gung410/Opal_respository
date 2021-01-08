using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Commands
{
    public class AwardBadgeCommand : BaseThunderCommand
    {
        public List<Guid> UserIds { get; set; }

        public Guid BadgeId { get; set; }

        public Guid? CurrentUserId { get; set; }
    }
}
