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

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveCoursePlanningCycleCommandHandler : BaseCommandHandler<SaveCoursePlanningCycleCommand>
    {
        private readonly IReadOnlyRepository<CoursePlanningCycle> _readCoursePlanningCycleRepository;
        private readonly CoursePlanningCycleCudLogic _coursePlanningCycleCudLogic;

        public SaveCoursePlanningCycleCommandHandler(
            IReadOnlyRepository<CoursePlanningCycle> readCoursePlanningCycleRepository,
            CoursePlanningCycleCudLogic coursePlanningCycleCudLogic,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCoursePlanningCycleRepository = readCoursePlanningCycleRepository;
            _coursePlanningCycleCudLogic = coursePlanningCycleCudLogic;
        }

        protected override async Task HandleAsync(SaveCoursePlanningCycleCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreate)
            {
                await CreateNew(command, cancellationToken);
            }
            else
            {
                await Update(command, cancellationToken);
            }
        }

        private async Task CreateNew(SaveCoursePlanningCycleCommand command, CancellationToken cancellationToken)
        {
            var coursePlanningCycle = new CoursePlanningCycle
            {
                Id = command.Id
            };

            EnsureValidPermission(CoursePlanningCycle.HasVerificationPermission(CurrentUserId, CurrentUserRoles));

            SetDataForCoursePlanningCycleEntity(coursePlanningCycle, command);
            coursePlanningCycle.CreatedDate = Clock.Now;
            coursePlanningCycle.CreatedBy = CurrentUserIdOrDefault;

            await _coursePlanningCycleCudLogic.Insert(coursePlanningCycle, cancellationToken);
        }

        private async Task Update(SaveCoursePlanningCycleCommand command, CancellationToken cancellationToken)
        {
            var coursePlanningCycle = await _readCoursePlanningCycleRepository.GetAsync(command.Id);

            EnsureValidPermission(CoursePlanningCycle.HasVerificationPermission(CurrentUserId, CurrentUserRoles));

            SetDataForCoursePlanningCycleEntity(coursePlanningCycle, command);
            coursePlanningCycle.ChangedDate = Clock.Now;
            coursePlanningCycle.ChangedBy = CurrentUserId;

            await _coursePlanningCycleCudLogic.Update(coursePlanningCycle, cancellationToken);
        }

        private void SetDataForCoursePlanningCycleEntity(CoursePlanningCycle coursePlanningCycle, SaveCoursePlanningCycleCommand command)
        {
            coursePlanningCycle.YearCycle = command.YearCycle;
            coursePlanningCycle.Title = command.Title;
            coursePlanningCycle.StartDate = command.StartDate;
            coursePlanningCycle.EndDate = command.EndDate;
            coursePlanningCycle.Description = command.Description;
        }
    }
}
