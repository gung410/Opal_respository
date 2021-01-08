using Microservice.Badge.Domain.ValueObjects;

namespace Microservice.Badge.Application.RequestDtos
{
    public class SaveActiveContributorBadgeCriteriaRequest
    {
        public ActiveContributorsBadgeCriteria Criteria { get; set; }
    }
}
