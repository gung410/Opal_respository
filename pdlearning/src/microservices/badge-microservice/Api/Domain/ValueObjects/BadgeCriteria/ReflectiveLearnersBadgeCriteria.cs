using Microservice.Badge.Attributes;
using Microservice.Badge.Domain.Constants;

namespace Microservice.Badge.Domain.ValueObjects
{
    [BadgeCriteriaFor(BadgeIdsConstants.ReflectiveLearnersBadgeIdStr)]
    public class ReflectiveLearnersBadgeCriteria : BaseBadgeCriteria
    {
        public int SumOfReflection { get; set; }

        public int SumOfSharedReflection { get; set; }
    }
}
