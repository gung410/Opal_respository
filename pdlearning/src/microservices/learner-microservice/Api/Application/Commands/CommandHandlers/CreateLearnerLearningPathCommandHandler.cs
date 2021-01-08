using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class CreateLearnerLearningPathCommandHandler : BaseCommandHandler<CreateLearnerLearningPathCommand>
    {
        private readonly IRepository<LearnerLearningPath> _learnerLearningPathRepository;
        private readonly IRepository<LearnerLearningPathCourse> _learnerLearningPathCourseRepository;

        public CreateLearnerLearningPathCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<LearnerLearningPath> learnerLearningPathRepository,
            IRepository<LearnerLearningPathCourse> learnerLearningPathCourseRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _learnerLearningPathRepository = learnerLearningPathRepository;
            _learnerLearningPathCourseRepository = learnerLearningPathCourseRepository;
        }

        protected override async Task HandleAsync(CreateLearnerLearningPathCommand command, CancellationToken cancellationToken)
        {
            var learnerLearningPath = new LearnerLearningPath
            {
                Id = command.Id,
                Title = command.Title,
                CreatedBy = CurrentUserIdOrDefault,
                ThumbnailUrl = command.ThumbnailUrl
            };

            await _learnerLearningPathRepository.InsertAsync(learnerLearningPath);

            var learnerLearningPathCourses = command.Courses
                .Select(c => new LearnerLearningPathCourse
                {
                    Id = Guid.NewGuid(),
                    CourseId = c.CourseId,
                    LearningPathId = command.Id,
                    Order = c.Order
                })
                .ToList();

            await _learnerLearningPathCourseRepository.InsertManyAsync(learnerLearningPathCourses);
        }
    }
}
