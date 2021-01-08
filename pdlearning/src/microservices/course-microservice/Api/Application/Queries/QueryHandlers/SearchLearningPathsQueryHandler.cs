using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class SearchLearningPathsQueryHandler : BaseQueryHandler<SearchLearningPathsQuery, PagedResultDto<LearningPathModel>>
    {
        private readonly IReadOnlyRepository<LearningPath> _readLearningPathRepository;
        private readonly IReadOnlyRepository<LearningPathCourse> _readLearningPathCourseRepository;

        public SearchLearningPathsQueryHandler(
            IReadOnlyRepository<LearningPath> readLearningPathRepository,
            IReadOnlyRepository<LearningPathCourse> readLearningPathCoursesRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readLearningPathRepository = readLearningPathRepository;
            _readLearningPathCourseRepository = readLearningPathCoursesRepository;
        }

        protected override async Task<PagedResultDto<LearningPathModel>> HandleAsync(SearchLearningPathsQuery query, CancellationToken cancellationToken)
        {
            var totalLearningPathQuery = _readLearningPathRepository.GetAll().Where(_ => _.CreatedBy == CurrentUserId);

            totalLearningPathQuery = totalLearningPathQuery.WhereIf(!string.IsNullOrEmpty(query.SearchText), p => EF.Functions.FreeText(p.Title, query.SearchText));

            var learningPathQuery = ApplyPaging(totalLearningPathQuery, query.PageInfo);
            var learningPathCoursesQuery = _readLearningPathCourseRepository.GetAll().Join(learningPathQuery, p => p.LearningPathId, p => p.Id, (learningPathCourse, learningPath) => learningPathCourse).Distinct();
            var totalCount = await totalLearningPathQuery.CountAsync(cancellationToken);
            var learningPathModels = await BuildLearningPathModels(learningPathQuery, learningPathCoursesQuery, cancellationToken);

            return new PagedResultDto<LearningPathModel>(totalCount, learningPathModels.ToList());
        }

        private static async Task<IEnumerable<LearningPathModel>> BuildLearningPathModels(
            IQueryable<LearningPath> learningPathQuery,
            IQueryable<LearningPathCourse> learningPathCoursesQuery,
            CancellationToken cancellationToken)
        {
            var learningPaths = await learningPathQuery.ToListAsync(cancellationToken);
            var learningPathCoursesDic = (await learningPathCoursesQuery.ToListAsync(cancellationToken))
                .GroupBy(p => p.LearningPathId)
                .ToDictionary(p => p.Key, p => p.ToList());
            var learningPathModels = learningPaths.Select(p => LearningPathModel.Create(p, learningPathCoursesDic.ContainsKey(p.Id) ? learningPathCoursesDic[p.Id] : null));
            return learningPathModels;
        }
    }
}
