using Microservice.Badge.Attributes;
using Microservice.Badge.Domain.Constants;

namespace Microservice.Badge.Domain.ValueObjects
{
    [BadgeCriteriaFor(BadgeIdsConstants.VisualStorytellerBadgeIdStr)]
    public class VisualStorytellerBadgeCriteria : BaseBadgeCriteria
    {
        /// <summary>
        /// Post that contains more than {NumOfMultimedia} and has more than {NumOfResponse} responses.
        /// </summary>
        public int NumOfQualifiedVisualPost { get; set; }

        public int NumOfResponse { get; set; }

        public int NumOfMultimedia { get; set; }
    }
}
