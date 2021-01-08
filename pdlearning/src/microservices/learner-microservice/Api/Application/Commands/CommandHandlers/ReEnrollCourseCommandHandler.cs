using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class ReEnrollCourseCommandHandler : BaseCommandHandler<ReEnrollCourseCommand>
    {
        private readonly IWriteMyCourseLogic _myCourseCudLogic;
        private readonly IReadOnlyRepository<MyCourse> _readMyCourseRepository;
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCudLogic;
        private readonly IRepository<LectureInMyCourse> _lectureInMyCourseRepository;

        public ReEnrollCourseCommandHandler(
            IUserContext userContext,
            IWriteMyCourseLogic myCourseCudLogic,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<MyCourse> readMyCourseRepository,
            IWriteMyOutstandingTaskLogic myOutstandingTaskCudLogic,
            IRepository<LectureInMyCourse> lectureInMyCourseRepository) : base(unitOfWorkManager, userContext)
        {
            _myCourseCudLogic = myCourseCudLogic;
            _readMyCourseRepository = readMyCourseRepository;
            _myOutstandingTaskCudLogic = myOutstandingTaskCudLogic;
            _lectureInMyCourseRepository = lectureInMyCourseRepository;
        }

        protected override async Task HandleAsync(ReEnrollCourseCommand command, CancellationToken cancellationToken)
        {
            var existingMyCourse = await _readMyCourseRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.CourseId == command.CourseId)
                .Where(p => p.CourseType == command.CourseType)
                .OrderByDescending(p => p.CreatedDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingMyCourse == null)
            {
                throw new EntityNotFoundException();
            }

            if (!existingMyCourse.HasContentChanged)
            {
                throw new NoContentChangeException();
            }

            if (!command.LectureIds.Any())
            {
                throw new NoContentException();
            }

            await _myOutstandingTaskCudLogic.DeleteMicroLearningTask(existingMyCourse);

            switch (existingMyCourse.Status)
            {
                case MyCourseStatus.Completed:
                    await ReEnrollOnCourseCompleted(command);
                    break;
                case MyCourseStatus.InProgress:
                    await ReEnrollOnCourseInProgress(existingMyCourse, command);
                    break;
                default:
                    throw new InvalidStatusException();
            }
        }

        private async Task ReEnrollOnCourseCompleted(ReEnrollCourseCommand command)
        {
            var myCourse = new MyCourse();

            await InsertMyCourse(myCourse, command);
            await InsetManyLecture(command.Id, command);
        }

        private async Task ReEnrollOnCourseInProgress(
            MyCourse existingMyCourse,
            ReEnrollCourseCommand command)
        {
            var myCourse = new MyCourse();

            await InsertMyCourse(myCourse, command);
            await InsetManyLecture(command.Id, command);
            await DeleteLecturesByMyCourseId(existingMyCourse.UserId, existingMyCourse.Id);
        }

        private async Task InsertMyCourse(MyCourse myCourse, ReEnrollCourseCommand command)
        {
            var now = Clock.Now;
            myCourse.Id = command.Id;
            myCourse.StartDate = now;
            myCourse.LastLogin = now;
            myCourse.CreatedDate = now;
            myCourse.ProgressMeasure = 0;
            myCourse.CourseId = command.CourseId;
            myCourse.UserId = CurrentUserIdOrDefault;
            myCourse.CreatedBy = CurrentUserIdOrDefault;
            myCourse.Status = MyCourseStatus.InProgress;
            myCourse.CourseType = command.CourseType;

            await _myCourseCudLogic.Insert(myCourse);

            await _myOutstandingTaskCudLogic.DeleteMicroLearningTask(myCourse);
        }

        private async Task InsetManyLecture(Guid myCourseId, ReEnrollCourseCommand command)
        {
            var lectures = command.LectureIds.Select(lectureId => new LectureInMyCourse()
            {
                Id = Guid.NewGuid(),
                LectureId = lectureId,
                StartDate = Clock.Now,
                MyCourseId = myCourseId,
                UserId = CurrentUserIdOrDefault,
                Status = LectureStatus.NotStarted,
                CreatedBy = CurrentUserIdOrDefault,
            });

            await _lectureInMyCourseRepository.InsertManyAsync(lectures);
        }

        private async Task DeleteLecturesByMyCourseId(Guid userId, Guid myCourseId)
        {
            var lectures = await _lectureInMyCourseRepository
                .GetAll()
                .Where(p => p.UserId == userId)
                .Where(p => p.MyCourseId == myCourseId)
                .ToListAsync();

            await _lectureInMyCourseRepository.DeleteManyAsync(lectures);
        }
    }
}
