using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetLearningPathByIdQueryHandler : BaseQueryHandler<GetLearningPathByIdQuery, LearningPathModel>
    {
        private readonly IReadOnlyRepository<LearningPath> _readLearningPathRepository;
        private readonly IReadOnlyRepository<LearningPathCourse> _readLearningPathCourseRepository;

        public GetLearningPathByIdQueryHandler(
            IReadOnlyRepository<LearningPath> readLearningPathRepository,
            IReadOnlyRepository<LearningPathCourse> readLearningPathCoursesRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLearningPathRepository = readLearningPathRepository;
            _readLearningPathCourseRepository = readLearningPathCoursesRepository;
        }

        protected override async Task<LearningPathModel> HandleAsync(GetLearningPathByIdQuery query, CancellationToken cancellationToken)
        {
            var learningPath = await _readLearningPathRepository.GetAsync(query.Id);
            var learningPathCourses = await _readLearningPathCourseRepository.GetAllListAsync(_ => _.LearningPathId == query.Id);

            return LearningPathModel.Create(learningPath, learningPathCourses);
        }
    }
}
