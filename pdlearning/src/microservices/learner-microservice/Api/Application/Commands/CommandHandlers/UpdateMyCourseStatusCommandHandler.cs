using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class UpdateMyCourseStatusCommandHandler : BaseCommandHandler<UpdateMyCourseStatusCommand>
    {
        private readonly IWriteMyCourseLogic _myCourseCudLogic;
        private readonly IReadMyCourseShared _readMyCourseShared;
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCudLogic;

        public UpdateMyCourseStatusCommandHandler(
            IUserContext userContext,
            IWriteMyCourseLogic myCourseCudLogic,
            IUnitOfWorkManager unitOfWorkManager,
            IReadMyCourseShared readMyCourseShared,
            IWriteMyOutstandingTaskLogic myOutstandingTaskCudLogic) : base(unitOfWorkManager, userContext)
        {
            _myCourseCudLogic = myCourseCudLogic;
            _readMyCourseShared = readMyCourseShared;
            _myOutstandingTaskCudLogic = myOutstandingTaskCudLogic;
        }

        protected override async Task HandleAsync(UpdateMyCourseStatusCommand command, CancellationToken cancellationToken)
        {
            var existingMyCourse = await _readMyCourseShared
                .GetByUserIdAndCourseId(CurrentUserIdOrDefault, command.CourseId);

            if (existingMyCourse == null)
            {
                throw new EntityNotFoundException(typeof(MyCourse), command.CourseId);
            }

            ValidateStatus(command, existingMyCourse);

            await UpdateMyCourse(existingMyCourse, command);
        }

        private async Task UpdateMyCourse(MyCourse existingMyCourse, UpdateMyCourseStatusCommand command)
        {
            var now = Clock.Now;

            existingMyCourse.LastLogin = now;
            existingMyCourse.Status = command.Status;
            existingMyCourse.ChangedBy = CurrentUserId;

            if (command.Status == MyCourseStatus.Completed)
            {
                existingMyCourse.EndDate = now;
                existingMyCourse.CompletedDate = now;

                if (existingMyCourse.IsMicroLearning())
                {
                    await _myOutstandingTaskCudLogic.DeleteMicroLearningTask(existingMyCourse);
                }
            }

            await _myCourseCudLogic.Update(existingMyCourse);
        }

        private bool ValidateStatus(UpdateMyCourseStatusCommand command, MyCourse existingMyCourse)
        {
            if (!existingMyCourse.IsMicroLearning())
            {
                throw new BusinessLogicException("You cannot update status for this course.");
            }

            if (existingMyCourse.Status == MyCourseStatus.InProgress && command.Status == MyCourseStatus.Completed)
            {
                return true;
            }

            throw new InvalidStatusException();
        }
    }
}
