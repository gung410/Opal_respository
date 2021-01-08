using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;

namespace Microservice.Badge.Application.BusinessLogic
{
    public abstract class BaseBadgeCriteriaResolverLogic<TCriteria> where TCriteria : BaseBadgeCriteria
    {
        protected BaseBadgeCriteriaResolverLogic(
            BadgeDbContext dbContext,
            IGetBadgeCriteriaLogic<TCriteria> getBadgeCriteriaLogic)
        {
            BadgeDbContext = dbContext;
            GetBadgeCriteriaLogic = getBadgeCriteriaLogic;
        }

        protected BadgeDbContext BadgeDbContext { get; }

        protected IGetBadgeCriteriaLogic<TCriteria> GetBadgeCriteriaLogic { get; }
    }
}
