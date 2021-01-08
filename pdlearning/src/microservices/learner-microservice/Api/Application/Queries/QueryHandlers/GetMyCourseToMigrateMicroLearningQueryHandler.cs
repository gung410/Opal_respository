using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Common;
using Microservice.Learner.Application.Events;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyCourseToMigrateMicroLearningQueryHandler : BaseQueryHandler<GetMyCourseToMigrateMicroLearningQuery, int>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<MyCourse> _myCourseRepository;
        private readonly IRepository<UserReview> _userReviewRepository;

        public GetMyCourseToMigrateMicroLearningQueryHandler(
            IThunderCqrs thunderCqrs,
            IRepository<MyCourse> myCourseRepository,
            IRepository<UserReview> userReviewRepository,
            IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _myCourseRepository = myCourseRepository;
            _userReviewRepository = userReviewRepository;
        }

        protected override Task<int> HandleAsync(GetMyCourseToMigrateMicroLearningQuery query, CancellationToken cancellationToken)
        {
            if (!UserContext.IsSysAdministrator())
            {
                throw new UnauthorizedAccessException("The user must be in role SysAdministrator!");
            }

            if (query.BatchSize == 0)
            {
                throw new BusinessLogicException("Please specify the batch size");
            }

            if (query.Statuses.Any(status => status == MyCourseStatus.Passed || status == MyCourseStatus.Failed))
            {
                throw new BusinessLogicException("Invalid status.");
            }

            if (query.CourseIds != null && !query.CourseIds.Any())
            {
                throw new BusinessLogicException("CourseIds is empty.");
            }

            if (query.CourseIds != null && query.CourseIds.Any())
            {
                return MigrateWithSpecificCourseIds(query, cancellationToken);
            }

            return MigrateAllMicroLearning(query, cancellationToken);
        }

        private async Task<int> MigrateWithSpecificCourseIds(GetMyCourseToMigrateMicroLearningQuery query, CancellationToken cancellationToken)
        {
            var batchSize = query.BatchSize;
            int processedItems = 0;

            for (int i = 0; i < query.CourseIds.Count; i += query.BatchSize)
            {
                var commandCourseIds = query.CourseIds
                    .Skip(i)
                    .Take(batchSize)
                    .ToList();

                var myCourses = await _myCourseRepository
                    .GetAll()
                    .Where(p => p.CourseType == LearningCourseType.Microlearning)
                    .Where(p => query.Statuses.Contains(p.Status))
                    .Where(p => commandCourseIds.Contains(p.CourseId))
                    .ToListAsync(cancellationToken);

                processedItems += myCourses.Count;

                batchSize += query.BatchSize;

                await SendLearningRecordEvent(query, myCourses, cancellationToken);
            }

            return processedItems;
        }

        private async Task<int> MigrateAllMicroLearning(GetMyCourseToMigrateMicroLearningQuery query, CancellationToken cancellationToken)
        {
            var totalCount = await _myCourseRepository
                .GetAll()
                .Where(p => p.CourseType == LearningCourseType.Microlearning)
                .CountAsync(cancellationToken);

            var batchSize = query.BatchSize;
            int processedItems = 0;

            for (int i = 0; i < totalCount; i += query.BatchSize)
            {
                var myCourses = await _myCourseRepository
                    .GetAll()
                    .Where(p => p.CourseType == LearningCourseType.Microlearning)
                    .Where(p => query.Statuses.Contains(p.Status))
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip(i)
                    .Take(batchSize)
                    .ToListAsync(cancellationToken);

                processedItems += myCourses.Count;

                batchSize += query.BatchSize;

                await SendLearningRecordEvent(query, myCourses, cancellationToken);
            }

            return processedItems;
        }

        private async Task SendLearningRecordEvent(GetMyCourseToMigrateMicroLearningQuery query, List<MyCourse> myCourses, CancellationToken cancellationToken)
        {
            var courseIds = myCourses.Select(p => p.CourseId).ToList();

            var userReviews = await _userReviewRepository
                .GetAll()
                .Where(p => courseIds.Contains(p.ItemId))
                .ToListAsync(cancellationToken);

            foreach (var myCourse in myCourses)
            {
                var userReview = userReviews.FirstOrDefault(p => p.ItemId == myCourse.CourseId && p.UserId == myCourse.UserId);

                switch (query.MigrationEventType)
                {
                    case MigrationEventType.All:
                        await _thunderCqrs.SendEvent(new MyCourseChangeEvent(myCourse, myCourse.Status), cancellationToken);
                        await _thunderCqrs.SendEvent(new LearningRecordEvent(myCourse, userReview), cancellationToken);
                        break;

                    case MigrationEventType.LearningHistory:
                        // Support  for search module
                        await _thunderCqrs.SendEvent(new MyCourseChangeEvent(myCourse, myCourse.Status), cancellationToken);
                        break;

                    case MigrationEventType.LearningRecord:
                        // Support for report module
                        await _thunderCqrs.SendEvent(
                            new LearningRecordEvent(myCourse, userReview), cancellationToken);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
