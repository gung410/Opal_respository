using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class DeleteLearnerLearningPathCommandHandler : BaseCommandHandler<DeleteLearnerLearningPathCommand>
    {
        private readonly IRepository<UserBookmark> _userBookmarkRepository;
        private readonly IRepository<LearnerLearningPath> _learnerLearningPathRepository;
        private readonly IRepository<LearnerLearningPathCourse> _learnerLearningPathCourseRepository;

        public DeleteLearnerLearningPathCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserBookmark> userBookmarkRepository,
            IRepository<LearnerLearningPath> learnerLearningPathRepository,
            IRepository<LearnerLearningPathCourse> learnerLearningPathCourseRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _userBookmarkRepository = userBookmarkRepository;
            _learnerLearningPathRepository = learnerLearningPathRepository;
            _learnerLearningPathCourseRepository = learnerLearningPathCourseRepository;
        }

        protected override async Task HandleAsync(DeleteLearnerLearningPathCommand command, CancellationToken cancellationToken)
        {
            var learningPath = await _learnerLearningPathRepository.GetAsync(command.Id);

            var learningPathCourses = await _learnerLearningPathCourseRepository
                .GetAll()
                .Where(p => p.LearningPathId == command.Id)
                .ToListAsync(cancellationToken);

            var userBookmark = await _userBookmarkRepository
                .GetAll()
                .Where(p => p.ItemId == command.Id)
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.ItemType == BookmarkType.LearningPath)
                .FirstOrDefaultAsync(cancellationToken);

            if (userBookmark != null)
            {
                await _userBookmarkRepository.DeleteAsync(userBookmark);
            }

            await _learnerLearningPathRepository.DeleteAsync(learningPath);
            await _learnerLearningPathCourseRepository.DeleteManyAsync(learningPathCourses);
        }
    }
}
