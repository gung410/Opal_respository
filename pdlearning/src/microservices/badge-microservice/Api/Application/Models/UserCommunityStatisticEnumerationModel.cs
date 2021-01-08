using System;
using System.Collections.Generic;
using Microservice.Badge.Domain.Enums;

namespace Microservice.Badge.Application.Models
{
    public class UserCommunityStatisticEnumerationModel
    {
        public Guid UserId { get; set; }

        public Guid CommunityId { get; set; }

        public IEnumerable<CommunityActivityEnumerationModel> CommunityActivities { get; set; }
    }

    public class CommunityActivityEnumerationModel
    {
        public ActivityType Type { get; set; }

        public string SourceId { get; set; }

        public int PostId { get; set; }
    }
}
