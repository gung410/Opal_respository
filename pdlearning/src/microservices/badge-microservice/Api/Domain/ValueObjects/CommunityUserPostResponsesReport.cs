using System;
using System.Collections.Generic;

namespace Microservice.Badge.Domain.ValueObjects
{
    public class CommunityUserPostResponsesReport
    {
        public Guid UserId { get; set; }

        public IEnumerable<CommunityPost> Communities { get; set; }
    }
}
