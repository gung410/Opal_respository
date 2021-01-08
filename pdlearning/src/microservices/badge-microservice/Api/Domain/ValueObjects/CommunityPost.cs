using System;
using System.Collections.Generic;

namespace Microservice.Badge.Domain.ValueObjects
{
    public class CommunityPost
    {
        public Guid CommunityId { get; set; }

        public int NumOfPosts { get; set; }

        public IEnumerable<CommunityPostResponses> Posts { get; set; }
    }
}
