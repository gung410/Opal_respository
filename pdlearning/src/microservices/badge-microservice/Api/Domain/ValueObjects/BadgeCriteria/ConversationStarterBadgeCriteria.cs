using Microservice.Badge.Attributes;
using Microservice.Badge.Domain.Constants;

namespace Microservice.Badge.Domain.ValueObjects
{
    [BadgeCriteriaFor(BadgeIdsConstants.ConversationStarterBadgeIdStr)]
    public class ConversationStarterBadgeCriteria : BaseBadgeCriteria
    {
        public int NumOfQualifiedPost { get; init; }

        public int NumOfResponse { get; init; }
    }
}
