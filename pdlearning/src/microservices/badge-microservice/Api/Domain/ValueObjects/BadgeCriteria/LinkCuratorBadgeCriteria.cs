using Microservice.Badge.Attributes;
using Microservice.Badge.Domain.Constants;

namespace Microservice.Badge.Domain.ValueObjects
{
    [BadgeCriteriaFor(BadgeIdsConstants.LinkCuratorBadgeIdStr)]
    public class LinkCuratorBadgeCriteria : BaseBadgeCriteria
    {
        /// <summary>
        /// Post that contains links and has more than 10 responses.
        /// </summary>
        public int NumOfQualifiedLinkCuratorPost { get; set; }

        public int NumOfResponse { get; set; }
    }
}
