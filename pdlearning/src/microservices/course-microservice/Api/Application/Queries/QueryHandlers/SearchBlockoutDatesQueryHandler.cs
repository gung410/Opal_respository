using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class SearchBlockoutDatesQueryHandler : BaseQueryHandler<SearchBlockoutDatesQuery, PagedResultDto<BlockoutDateModel>>
    {
        private readonly IReadOnlyRepository<BlockoutDate> _readBlockoutDateRepository;
        private readonly GetFullTextFilteredEntitiesSharedQuery _getFullTextFilteredEntitiesSharedQuery;

        public SearchBlockoutDatesQueryHandler(
            IReadOnlyRepository<BlockoutDate> readBlockoutDateRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            GetFullTextFilteredEntitiesSharedQuery getFullTextFilteredEntitiesSharedQuery,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readBlockoutDateRepository = readBlockoutDateRepository;
            _getFullTextFilteredEntitiesSharedQuery = getFullTextFilteredEntitiesSharedQuery;
        }

        protected override async Task<PagedResultDto<BlockoutDateModel>> HandleAsync(SearchBlockoutDatesQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readBlockoutDateRepository.GetAll();

            dbQuery = dbQuery.Where(p => p.PlanningCycleId == query.CoursePlanningCycleId);

            dbQuery = _getFullTextFilteredEntitiesSharedQuery.BySearchText(dbQuery, query.SearchText, p => p.FullTextSearch);

            dbQuery = query.Filter?.ContainFilters.Aggregate(
                dbQuery,
                (current, containFilter) =>
                    current.BuildContainFilter(
                        containFilter.Field,
                        containFilter.Values,
                        null,
                        containFilter.NotContain,
                        (values, fieldName, currentQuery) => _getFullTextFilteredEntitiesSharedQuery.ByFilter(
                            currentQuery,
                            fieldName,
                            values,
                            null))) ?? dbQuery;

            dbQuery = query.Filter?.FromToFilters?.Aggregate(
                dbQuery,
                (current, fromToFilter) =>
                    current.BuildFromToFilter(
                        fromToFilter.Field,
                        fromToFilter.FromValue,
                        fromToFilter.ToValue,
                        fromToFilter.EqualFrom,
                        fromToFilter.EqualTo)) ?? dbQuery;

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            if (query.PageInfo != null && query.PageInfo.MaxResultCount == 0)
            {
                return new PagedResultDto<BlockoutDateModel>(totalCount);
            }

            dbQuery = dbQuery.OrderByDescending(p => p.FullTextSearchKey);

            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            return new PagedResultDto<BlockoutDateModel>(totalCount, dbQuery.Select(p => new BlockoutDateModel(p)).ToList());
        }
    }
}
