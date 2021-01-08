using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class UpdateLearningTrackingCommandHandler : BaseCommandHandler<UpdateLearningTrackingCommand>
    {
        private readonly IDbContextProvider<LearnerDbContext> _dbContextProvider;

        public UpdateLearningTrackingCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IDbContextProvider<LearnerDbContext> dbContextProvider,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _dbContextProvider = dbContextProvider;
        }

        protected override async Task HandleAsync(UpdateLearningTrackingCommand command, CancellationToken cancellationToken)
        {
            // To support can increase the total number of views of the user when accessing at the same time.
            var dbContext = _dbContextProvider.GetDbContext();
            var parameters = new[] { $"{command.ItemId}", $"{command.TrackingType}", $"{command.TrackingAction}" };
            await dbContext.Database.ExecuteSqlRawAsync("EXECUTE TrackingLearningActivity @p0,@p1,@p2", parameters, cancellationToken);
        }
    }
}
