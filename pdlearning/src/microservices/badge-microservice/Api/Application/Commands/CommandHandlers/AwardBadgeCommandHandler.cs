using System;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Badge.Application.BusinessLogic;
using Microservice.Badge.Infrastructure;

namespace Microservice.Badge.Application.Commands.CommandHandlers
{
    public class AwardBadgeCommandHandler : BaseCommandHandler<AwardBadgeCommand>
    {
        private readonly IProcessBadgeLogic _processBadgeLogic;

        public AwardBadgeCommandHandler(
            BadgeDbContext dbContext,
            IProcessBadgeLogic processBadgeLogic) : base(dbContext)
        {
            _processBadgeLogic = processBadgeLogic;
        }

        protected override Task HandleAsync(AwardBadgeCommand command, CancellationToken cancellationToken)
        {
            return _processBadgeLogic.IssueBadges(command.BadgeId, command.UserIds, command.CurrentUserId ?? Guid.Empty, cancellationToken);
        }
    }
}
