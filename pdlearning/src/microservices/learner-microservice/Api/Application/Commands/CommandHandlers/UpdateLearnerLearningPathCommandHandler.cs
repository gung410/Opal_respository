using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class UpdateLearnerLearningPathCommandHandler : BaseCommandHandler<UpdateLearnerLearningPathCommand>
    {
        private readonly IRepository<LearnerLearningPath> _learnerLearningPathRepository;
        private readonly IRepository<LearnerLearningPathCourse> _learnerLearningPathCourseRepository;

        public UpdateLearnerLearningPathCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<LearnerLearningPath> learnerLearningPathRepository,
            IRepository<LearnerLearningPathCourse> learnerLearningPathCourseRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _learnerLearningPathRepository = learnerLearningPathRepository;
            _learnerLearningPathCourseRepository = learnerLearningPathCourseRepository;
        }

        protected override async Task HandleAsync(UpdateLearnerLearningPathCommand command, CancellationToken cancellationToken)
        {
            var currentLearningPath = await _learnerLearningPathRepository
                .GetAll()
                .Where(f => f.Id == command.Id)
                .Where(f => f.CreatedBy == CurrentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentLearningPath == null)
            {
                throw new EntityNotFoundException(typeof(LearnerLearningPath), command.Id);
            }

            currentLearningPath.Title = command.Title;
            currentLearningPath.ThumbnailUrl = command.ThumbnailUrl;

            var currentLearningPathCourses = await _learnerLearningPathCourseRepository.GetAllListAsync(p => p.LearningPathId == currentLearningPath.Id);
            var currentLearningPathCoursesDic = currentLearningPathCourses.ToDictionary(p => p.Id);

            var updatedLearningPathCourses = command.Courses
               .Where(p => p.Id.HasValue)
               .Select(p =>
               {
                   p.UpdateExistedLearningPathCourse(currentLearningPathCoursesDic[p.Id.Value]);
                   return currentLearningPathCoursesDic[p.Id.Value];
               })
               .ToList();

            var toInsertLearningPathCourses = command.Courses
               .Where(p => p.Id == null)
               .Select(p => p.CreateNewLearningPathCourse(currentLearningPath.Id, p.CourseId, p.Order.Value))
               .ToList();

            var listLearningPathCourseIds = command.Courses.Where(_ => _.Id.HasValue).Select(_ => _.Id).ToList();
            var toDeleteLearningPathCourses = currentLearningPathCourses
                .Where(p => !listLearningPathCourseIds.Contains(p.Id));

            await _learnerLearningPathRepository.UpdateAsync(currentLearningPath);
            await _learnerLearningPathCourseRepository.DeleteManyAsync(toDeleteLearningPathCourses);
            await _learnerLearningPathCourseRepository.UpdateManyAsync(updatedLearningPathCourses);
            await _learnerLearningPathCourseRepository.InsertManyAsync(toInsertLearningPathCourses);
        }
    }
}
