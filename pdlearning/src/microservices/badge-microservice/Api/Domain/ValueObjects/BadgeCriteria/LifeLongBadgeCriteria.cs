using Microservice.Badge.Attributes;
using Microservice.Badge.Domain.Constants;

namespace Microservice.Badge.Domain.ValueObjects
{
    [BadgeCriteriaFor(BadgeIdsConstants.LifeLongBadgeIdStr)]
    public class LifeLongBadgeCriteria : BaseBadgeCriteria
    {
    }
}
