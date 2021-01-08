using System.Collections.Generic;
using System.Linq;
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
    public class GetLearningPathByIdsQueryHandler : BaseQueryHandler<GetLearningPathByIdsQuery, List<LearningPathModel>>
    {
        private readonly IReadOnlyRepository<LearningPath> _readLearningPathRepository;
        private readonly IReadOnlyRepository<LearningPathCourse> _readLearningPathCourseRepository;

        public GetLearningPathByIdsQueryHandler(
            IReadOnlyRepository<LearningPath> readLearningPathRepository,
            IReadOnlyRepository<LearningPathCourse> readLearningPathCoursesRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLearningPathRepository = readLearningPathRepository;
            _readLearningPathCourseRepository = readLearningPathCoursesRepository;
        }

        protected override async Task<List<LearningPathModel>> HandleAsync(GetLearningPathByIdsQuery query, CancellationToken cancellationToken)
        {
            var learningPaths = await _readLearningPathRepository.GetAllListAsync(p => query.Ids.Contains(p.Id));
            var learningPathCourses = await _readLearningPathCourseRepository.GetAllListAsync(_ => query.Ids.Contains(_.LearningPathId));

            return CreateListLearningPaths(learningPaths, learningPathCourses);
        }

        private List<LearningPathModel> CreateListLearningPaths(List<LearningPath> learningPaths, List<LearningPathCourse> learningPathCourses)
        {
            var listLearningPaths = new List<LearningPathModel>();
            foreach (var item in learningPaths)
            {
                var courseItemsByLearningPath = learningPathCourses.Where(p => p.LearningPathId == item.Id);
                var learningPath = LearningPathModel.Create(item, courseItemsByLearningPath);
                listLearningPaths.Add(learningPath);
            }

            return listLearningPaths;
        }
    }
}
