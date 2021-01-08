using Microservice.Badge.Application.Models;
using Microservice.Badge.Domain.ValueObjects;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Queries
{
    public class GetActiveContributorBadgeQuery : BaseThunderQuery<BadgeWithCriteriaModel<ActiveContributorsBadgeCriteria>>
    {
    }
}
