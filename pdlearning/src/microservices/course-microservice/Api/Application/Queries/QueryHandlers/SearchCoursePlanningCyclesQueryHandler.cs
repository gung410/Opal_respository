using System;
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
    public class SearchCoursePlanningCyclesQueryHandler : BaseQueryHandler<SearchCoursePlanningCyclesQuery, PagedResultDto<CoursePlanningCycleModel>>
    {
        private readonly IReadOnlyRepository<CoursePlanningCycle> _readCoursePlanningCycleRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public SearchCoursePlanningCyclesQueryHandler(
            IReadOnlyRepository<CoursePlanningCycle> readCoursePlanningCycleRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCoursePlanningCycleRepository = readCoursePlanningCycleRepository;
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task<PagedResultDto<CoursePlanningCycleModel>> HandleAsync(SearchCoursePlanningCyclesQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readCoursePlanningCycleRepository.GetAll();

            dbQuery = dbQuery.WhereIf(!string.IsNullOrEmpty(query.SearchText), r => !string.IsNullOrEmpty(r.Title) && EF.Functions.Like(r.Title, $"%{query.SearchText}%"));

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);
            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            var entities = await dbQuery.ToListAsync(cancellationToken);

            var courseDictionary = await GetAllCourseOfCoursePlanningCycleAsync(entities.Select(x => x.Id).ToList(), cancellationToken);

            return new PagedResultDto<CoursePlanningCycleModel>(totalCount, entities.Select(x => new CoursePlanningCycleModel(x, courseDictionary.ContainsKey(x.Id) ? courseDictionary[x.Id] : 0)).ToList());
        }

        private async Task<Dictionary<Guid?, int>> GetAllCourseOfCoursePlanningCycleAsync(List<Guid> coursePlanningCycleIds, CancellationToken cancellationToken)
        {
            return (await _readCourseRepository
                .GetAll()
                .Where(x => coursePlanningCycleIds.Contains(x.CoursePlanningCycleId.Value))
                .ToListAsync(cancellationToken)).GroupBy(x => x.CoursePlanningCycleId).ToDictionary(x => x.Key, x => x.Count());
        }
    }
}
