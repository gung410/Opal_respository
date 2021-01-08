using System;
using System.Collections.Generic;

namespace Microservice.Badge.Application.RequestDtos
{
    public class RewardUserBadgesRequest
    {
        public Guid BadgeId { get; set; }

        public List<Guid> UserIds { get; set; }
    }
}
