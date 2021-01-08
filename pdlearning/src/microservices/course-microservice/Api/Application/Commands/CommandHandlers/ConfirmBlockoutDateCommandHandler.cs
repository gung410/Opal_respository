using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ConfirmBlockoutDateCommandHandler : BaseCommandHandler<ConfirmBlockoutDateCommand>
    {
        private readonly IReadOnlyRepository<BlockoutDate> _readBlockoutDateRepository;
        private readonly IReadOnlyRepository<CoursePlanningCycle> _readCoursePlanningCycleRepository;
        private readonly CoursePlanningCycleCudLogic _coursePlanningCycleCudLogic;
        private readonly BlockoutDateCudLogic _blockoutDateCudLogic;
        private readonly ValidateConfirmBlockoutDateLogic _validateConfirmBlockoutDateLogic;

        public ConfirmBlockoutDateCommandHandler(
           IUnitOfWorkManager unitOfWorkManager,
           IReadOnlyRepository<BlockoutDate> readBlockoutDateRepository,
           IReadOnlyRepository<CoursePlanningCycle> readCoursePlanningCycleRepository,
           CoursePlanningCycleCudLogic coursePlanningCycleCudLogic,
           BlockoutDateCudLogic blockoutDateCudLogic,
           IUserContext userContext,
           IAccessControlContext<CourseUser> accessControlContext,
           ValidateConfirmBlockoutDateLogic validateConfirmBlockoutDateLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readBlockoutDateRepository = readBlockoutDateRepository;
            _readCoursePlanningCycleRepository = readCoursePlanningCycleRepository;
            _coursePlanningCycleCudLogic = coursePlanningCycleCudLogic;
            _blockoutDateCudLogic = blockoutDateCudLogic;
            _validateConfirmBlockoutDateLogic = validateConfirmBlockoutDateLogic;
        }

        protected override async Task HandleAsync(ConfirmBlockoutDateCommand command, CancellationToken cancellationToken)
        {
            EnsureValidPermission(BlockoutDate.HasCrudPermission(CurrentUserId, CurrentUserRoles));

            var coursePlanningCycle = await _readCoursePlanningCycleRepository.GetAsync(command.CoursePlanningCycleId);
            var blockoutDates = await _readBlockoutDateRepository
               .GetAll()
               .Where(p => p.PlanningCycleId == command.CoursePlanningCycleId)
               .ToListAsync(cancellationToken);

            EnsureBusinessLogicValid(_validateConfirmBlockoutDateLogic.Validate(blockoutDates));

            blockoutDates.ForEach(blockoutDate => blockoutDate.IsConfirmed = true);
            coursePlanningCycle.IsConfirmedBlockoutDate = true;

            await _coursePlanningCycleCudLogic.Update(coursePlanningCycle, cancellationToken);
            await _blockoutDateCudLogic.UpdateMany(blockoutDates, cancellationToken);
        }
    }
}
