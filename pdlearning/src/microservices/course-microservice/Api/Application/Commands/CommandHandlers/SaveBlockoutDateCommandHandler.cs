using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands
{
    public class SaveBlockoutDateCommandHandler : BaseCommandHandler<SaveBlockoutDateCommand>
    {
        private readonly IReadOnlyRepository<BlockoutDate> _readBlockoutDateRepository;
        private readonly IReadOnlyRepository<CoursePlanningCycle> _coursePlanningCycleRepository;
        private readonly BlockoutDateCudLogic _blockoutDateCudLogic;

        public SaveBlockoutDateCommandHandler(
            IReadOnlyRepository<BlockoutDate> readBlockoutDateRepository,
            BlockoutDateCudLogic blockoutDateCudLogic,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IReadOnlyRepository<CoursePlanningCycle> coursePlanningCycleRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readBlockoutDateRepository = readBlockoutDateRepository;
            _blockoutDateCudLogic = blockoutDateCudLogic;
            _coursePlanningCycleRepository = coursePlanningCycleRepository;
        }

        protected override async Task HandleAsync(SaveBlockoutDateCommand command, CancellationToken cancellationToken)
        {
            EnsureValidPermission(BlockoutDate.HasCrudPermission(CurrentUserId, CurrentUserRoles));

            if (command.IsCreate)
            {
                await CreateNew(command, cancellationToken);
            }
            else
            {
                await Update(command, cancellationToken);
            }
        }

        private async Task CreateNew(SaveBlockoutDateCommand command, CancellationToken cancellationToken)
        {
            var blockoutDate = new BlockoutDate
            {
                Id = command.Id
            };

            await SetDataForBlockoutDateEntity(blockoutDate, command);
            blockoutDate.CreatedDate = Clock.Now;
            blockoutDate.CreatedBy = CurrentUserIdOrDefault;

            await _blockoutDateCudLogic.Insert(blockoutDate, cancellationToken);
        }

        private async Task Update(SaveBlockoutDateCommand command, CancellationToken cancellationToken)
        {
            var blockoutDate = await _readBlockoutDateRepository.GetAsync(command.Id);

            await SetDataForBlockoutDateEntity(blockoutDate, command);
            blockoutDate.CreatedDate = Clock.Now;
            blockoutDate.CreatedBy = CurrentUserIdOrDefault;

            await _blockoutDateCudLogic.Update(blockoutDate, cancellationToken);
        }

        private async Task SetDataForBlockoutDateEntity(BlockoutDate blockoutDate, SaveBlockoutDateCommand command)
        {
            var planningCycle = await _coursePlanningCycleRepository.GetAsync(command.PlanningCycleId);

            blockoutDate.Title = command.Title;
            blockoutDate.Description = command.Description;
            blockoutDate.StartDay = command.StartDay;
            blockoutDate.StartMonth = command.StartMonth;
            blockoutDate.EndDay = command.EndDay;
            blockoutDate.EndMonth = command.EndMonth;
            blockoutDate.PlanningCycleId = command.PlanningCycleId;
            blockoutDate.ValidYear = planningCycle.YearCycle;
            blockoutDate.ServiceSchemes = command.ServiceSchemes;
            blockoutDate.Status = command.Status;
        }
    }
}
