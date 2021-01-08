using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;

namespace Microservice.Badge.Application.Commands.CommandHandlers
{
    public class SaveActiveContributorBadgeCriteriaCommandHandler : BaseCommandHandler<SaveActiveContributorBadgeCriteriaCommand>
    {
        public SaveActiveContributorBadgeCriteriaCommandHandler(
            BadgeDbContext dbContext) : base(dbContext)
        {
        }

        protected override async Task HandleAsync(SaveActiveContributorBadgeCriteriaCommand command, CancellationToken cancellationToken)
        {
            var filter = Builders<BadgeWithCriteria<ActiveContributorsBadgeCriteria>>.Filter.Eq(nameof(BadgeWithCriteria<ActiveContributorsBadgeCriteria>.Id), BadgeIdsConstants._activeContributorBadgeId);

            var update = Builders<BadgeWithCriteria<ActiveContributorsBadgeCriteria>>.Update.Set(nameof(BadgeWithCriteria<ActiveContributorsBadgeCriteria>.Criteria), command.Criteria);

            await DbContext.GetBadgeCriteriaCollection<ActiveContributorsBadgeCriteria>()
                .UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}
