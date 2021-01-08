using Microservice.Badge.Domain.ValueObjects;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Commands
{
    public class SaveActiveContributorBadgeCriteriaCommand : BaseThunderCommand
    {
        public ActiveContributorsBadgeCriteria Criteria { get; set; }
    }
}
