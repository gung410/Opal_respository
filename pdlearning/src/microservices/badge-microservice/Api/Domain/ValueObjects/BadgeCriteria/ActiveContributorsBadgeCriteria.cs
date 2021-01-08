using System;
using System.Collections.Generic;
using Microservice.Badge.Attributes;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Domain.Enums;
using Thunder.Platform.Core.Timing;

namespace Microservice.Badge.Domain.ValueObjects
{
    [BadgeCriteriaFor(BadgeIdsConstants.ActiveContributorBadgeIdStr)]
    public class ActiveContributorsBadgeCriteria : BaseBadgeCriteria
    {
        public BadgeLevelEnum? LevelOfCollaborativeLearnersBadge { get; set; }

        public BadgeLevelEnum? LevelOfDigitalLearnersBadge { get; set; }

        public BadgeLevelEnum? LevelOfReflectiveLearnersBadge { get; set; }

        public IEnumerable<Guid> CommunityBadgesIds { get; set; } = new List<Guid>();

        public int ExecuteMonth { get; set; } = Clock.Now.Month != 12 ? Clock.Now.Month + 1 : 1;
    }
}
