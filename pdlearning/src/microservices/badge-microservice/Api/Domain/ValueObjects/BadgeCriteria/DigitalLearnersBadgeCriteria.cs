using Microservice.Badge.Attributes;
using Microservice.Badge.Domain.Constants;

namespace Microservice.Badge.Domain.ValueObjects
{
    [BadgeCriteriaFor(BadgeIdsConstants.DigitalLearnersBadgeIdStr)]
    public class DigitalLearnersBadgeCriteria : BaseBadgeCriteria
    {
        public int NumOfCompletedMLU { get; set; }

        public int NumOfCompletedDigitalResources { get; set; }

        public int NumOfCompletedElearning { get; set; }
    }
}
