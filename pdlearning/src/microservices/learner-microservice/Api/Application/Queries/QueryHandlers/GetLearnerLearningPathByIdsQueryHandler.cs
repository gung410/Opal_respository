using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetLearnerLearningPathByIdsQueryHandler : BaseQueryHandler<GetLearnerLearningPathByIdsQuery, List<LearnerLearningPathModel>>
    {
        private readonly IRepository<LearnerLearningPath> _learnerLearningPathRepository;
        private readonly IRepository<LearnerLearningPathCourse> _learnerLearningPathCourseRepository;
        private readonly IRepository<UserBookmark> _userBookmarkRepository;

        public GetLearnerLearningPathByIdsQueryHandler(
            IRepository<LearnerLearningPath> learnerLearningPathRepository,
            IRepository<UserBookmark> userBookmarkRepository,
            IRepository<LearnerLearningPathCourse> learnerLearningPathCourseRepository,
            IUserContext userContext) : base(userContext)
        {
            _learnerLearningPathRepository = learnerLearningPathRepository;
            _learnerLearningPathCourseRepository = learnerLearningPathCourseRepository;
            _userBookmarkRepository = userBookmarkRepository;
        }

        protected override async Task<List<LearnerLearningPathModel>> HandleAsync(GetLearnerLearningPathByIdsQuery query, CancellationToken cancellationToken)
        {
            if (query.Ids?.Length == 0)
            {
                return new List<LearnerLearningPathModel>();
            }

            var learningPaths = await _learnerLearningPathRepository
                .GetAll()
                .Where(p => query.Ids.Contains(p.Id))
                .ToListAsync(cancellationToken);

            var learningPathIds = learningPaths.Select(p => p.Id).ToList();

            var learningPathCourses = await _learnerLearningPathCourseRepository
                .GetAll()
                .Where(lpc => learningPathIds.Contains(lpc.LearningPathId))
                .ToListAsync(cancellationToken);

            var bookmarks = await _userBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => learningPathIds.Contains(p.ItemId))
                .ToListAsync(cancellationToken);

            return learningPaths.Select(lp =>
            {
                var bookmark = bookmarks.FirstOrDefault(p => p.ItemId == lp.Id);
                var courses = learningPathCourses
                    .Where(p => p.LearningPathId == lp.Id)
                    .ToList();

                return LearnerLearningPathModel.New(
                        lp.Id,
                        lp.Title,
                        lp.CreatedBy,
                        lp.ThumbnailUrl,
                        lp.IsPublic)
                    .WithBookmarkInfo(bookmark)
                    .WithLearningPathCourses(courses);
            }).ToList();
        }
    }
}
