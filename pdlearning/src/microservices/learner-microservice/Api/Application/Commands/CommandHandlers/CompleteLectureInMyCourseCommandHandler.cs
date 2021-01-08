using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class CompleteLectureInMyCourseCommandHandler : BaseCommandHandler<CompleteLectureInMyCourseCommand>
    {
        private readonly IWriteMyCourseLogic _myCourseCudLogic;
        private readonly IReadOnlyRepository<MyCourse> _readMyCourseRepository;
        private readonly IRepository<LectureInMyCourse> _lectureInMyCourseRepository;

        public CompleteLectureInMyCourseCommandHandler(
            IUserContext userContext,
            IWriteMyCourseLogic myCourseCudLogic,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<MyCourse> readMyCourseRepository,
            IRepository<LectureInMyCourse> lectureInMyCourseRepository) : base(unitOfWorkManager, userContext)
        {
            _readMyCourseRepository = readMyCourseRepository;
            _lectureInMyCourseRepository = lectureInMyCourseRepository;
            _myCourseCudLogic = myCourseCudLogic;
        }

        protected override async Task HandleAsync(CompleteLectureInMyCourseCommand command, CancellationToken cancellationToken)
        {
            var existingLecture = await _lectureInMyCourseRepository.GetAsync(command.LectureInMyCourseId);

            if (existingLecture.UserId != CurrentUserId)
            {
                throw new UnauthorizedAccessException();
            }

            await UpdateExistingLectureToCompleted(existingLecture);

            await UpdateExistingMyCourseOnLectureCompleted(existingLecture, cancellationToken);
        }

        /// <summary>
        /// To calculate how many percent of completion for a course that user is learning.
        /// </summary>
        /// <param name="lectures">The list of lectures in the course.</param>
        /// <returns>The learning progress percentage (e.g. 75%).</returns>
        private double CalculateCourseProgress(List<LectureInMyCourse> lectures)
        {
            var total = lectures.Count();
            var completed = lectures.Count(p => p.Status == LectureStatus.Completed);
            var progress = (double)completed / total * 100;
            return Math.Round(progress, 1);
        }

        private Task UpdateExistingLectureToCompleted(LectureInMyCourse existingLecture)
        {
            var now = Clock.Now;
            existingLecture.StartDate = now;
            existingLecture.LastLogin = now;
            existingLecture.ChangedBy = CurrentUserId;
            existingLecture.Status = LectureStatus.Completed;
            return _lectureInMyCourseRepository.UpdateAsync(existingLecture);
        }

        private async Task UpdateExistingMyCourseOnLectureCompleted(LectureInMyCourse existingLecture, CancellationToken cancellationToken)
        {
            var existingMyCourse = await _readMyCourseRepository.GetAsync(existingLecture.MyCourseId);

            var lectures = await _lectureInMyCourseRepository
                .GetAll()
                .Where(p => p.MyCourseId == existingMyCourse.Id)
                .ToListAsync(cancellationToken);

            existingMyCourse.LastLogin = Clock.Now;
            existingMyCourse.ChangedBy = CurrentUserId;
            existingMyCourse.CurrentLecture = existingLecture.LectureId;
            existingMyCourse.ProgressMeasure = CalculateCourseProgress(lectures);

            await _myCourseCudLogic.Update(existingMyCourse);
        }
    }
}
