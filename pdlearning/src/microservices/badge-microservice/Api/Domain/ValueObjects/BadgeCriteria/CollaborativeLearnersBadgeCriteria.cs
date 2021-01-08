using Microservice.Badge.Attributes;
using Microservice.Badge.Domain.Constants;

namespace Microservice.Badge.Domain.ValueObjects
{
    [BadgeCriteriaFor(BadgeIdsConstants.CollaborativeLearnersBadgeIdStr)]
    public class CollaborativeLearnersBadgeCriteria : BaseBadgeCriteria
    {
        public int SumOfForward { get; set; }

        public int SumOfFollow { get; set; }

        public int SumOfPostAndLike { get; set; }

        public int SumOfPostsResponded { get; set; }

        public int Total { get; set; }
    }
}
