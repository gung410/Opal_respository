using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Infrastructure;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.EntityFrameworkCore;

namespace Microservice.Content.Application.Commands.CommandHandlers
{
    public class UpdateLearningTrackingCommandHandler : BaseCommandHandler<UpdateLearningTrackingCommand>
    {
        private readonly IRepository<LearningTracking> _learningTrackingRepository;
        private readonly IDbContextProvider<ContentDbContext> _dbContextProvider;

        public UpdateLearningTrackingCommandHandler(
            IUserContext userContext,
            IAccessControlContext accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<LearningTracking> learningTrackingRepository,
            IDbContextProvider<ContentDbContext> dbContextProvider) : base(unitOfWorkManager, userContext, accessControlContext)
        {
            _dbContextProvider = dbContextProvider;
            _learningTrackingRepository = learningTrackingRepository;
        }

        protected override async Task HandleAsync(UpdateLearningTrackingCommand command, CancellationToken cancellationToken)
        {
            var learningTracking = await _learningTrackingRepository.FirstOrDefaultAsync(
                p => p.ItemId == command.ItemId
                && p.TrackingType == command.TrackingType
                && p.TrackingAction == command.TrackingAction);

            if (learningTracking != null)
            {
                learningTracking.TotalCount = learningTracking.TotalCount + 1;
                await _learningTrackingRepository.UpdateAsync(learningTracking);
            }
            else
            {
                await _learningTrackingRepository.InsertAsync(
                    new LearningTracking
                    {
                        ItemId = command.ItemId,
                        TrackingType = command.TrackingType,
                        TrackingAction = command.TrackingAction,
                        TotalCount = 1
                    });
            }
        }
    }
}
