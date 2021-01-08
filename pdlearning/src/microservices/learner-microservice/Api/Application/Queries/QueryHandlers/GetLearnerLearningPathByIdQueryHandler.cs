using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetLearnerLearningPathByIdQueryHandler : BaseQueryHandler<GetLearnerLearningPathByIdQuery, LearnerLearningPathModel>
    {
        private readonly IRepository<LearnerLearningPath> _learnerLearningPathRepository;
        private readonly IRepository<LearnerLearningPathCourse> _learnerLearningPathCourseRepository;
        private readonly IRepository<UserBookmark> _userBookmarkRepository;
        private readonly IRepository<UserSharing> _userSharingRepository;
        private readonly IRepository<UserSharingDetail> _userSharingDetailRepository;

        public GetLearnerLearningPathByIdQueryHandler(
            IRepository<LearnerLearningPath> learnerLearningPathRepository,
            IRepository<UserBookmark> userBookmarkRepository,
            IRepository<LearnerLearningPathCourse> learnerLearningPathCourseRepository,
            IRepository<UserSharing> userSharingRepository,
            IRepository<UserSharingDetail> userSharingDetailRepository,
            IUserContext userContext) : base(userContext)
        {
            _learnerLearningPathRepository = learnerLearningPathRepository;
            _learnerLearningPathCourseRepository = learnerLearningPathCourseRepository;
            _userBookmarkRepository = userBookmarkRepository;
            _userSharingRepository = userSharingRepository;
            _userSharingDetailRepository = userSharingDetailRepository;
        }

        protected override async Task<LearnerLearningPathModel> HandleAsync(GetLearnerLearningPathByIdQuery query, CancellationToken cancellationToken)
        {
            var learningPath = await _learnerLearningPathRepository.FirstOrDefaultAsync(lp => lp.Id == query.Id);
            if (learningPath == null)
            {
                throw new EntityNotFoundException(typeof(LearnerLearningPath), query.Id);
            }

            var isUserSharedLearningPath = await IsUserSharedLearningPath(query);

            // If user doesn't have access to learning path detail,
            // we need to return null instead of new LearnerLearningPath.
            // Support for UI doesn't need additional logic while getting learning path detail.
            if (!learningPath.IsPublic && !learningPath.IsCreationOwner(CurrentUserIdOrDefault) && !isUserSharedLearningPath)
            {
                return null;
            }

            var learningPathCourses = await _learnerLearningPathCourseRepository
                .GetAll()
                .Where(lpc => lpc.LearningPathId == query.Id)
                .OrderBy(o => o.Order)
                .ToListAsync(cancellationToken);

            var bookmark = await _userBookmarkRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemId == query.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return LearnerLearningPathModel.New(
                    learningPath.Id,
                    learningPath.Title,
                    learningPath.CreatedBy,
                    learningPath.ThumbnailUrl,
                    learningPath.IsPublic)
                .WithBookmarkInfo(bookmark)
                .WithLearningPathCourses(learningPathCourses);
        }

        private async Task<bool> IsUserSharedLearningPath(GetLearnerLearningPathByIdQuery query)
        {
            var sharingDetailQuery = _userSharingDetailRepository
                .GetAll();

            var userSharingQuery = _userSharingRepository
                .GetAll()
                .Where(p => p.ItemId == query.Id);

            var anyUserSharedLearningPath = await userSharingQuery
                .Join(
                    sharingDetailQuery,
                    userSharing => userSharing.Id,
                    sharingDetail => sharingDetail.UserSharingId,
                    (userSharing, sharingDetail) => sharingDetail)
                .Where(p => p.UserId == CurrentUserId)
                .AnyAsync();

            return anyUserSharedLearningPath;
        }
    }
}
