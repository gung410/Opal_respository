using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetAttendanceTrackingBySessionIdQueryHandler : BaseQueryHandler<GetAttendanceTrackingBySessionIdQuery, PagedResultDto<AttendanceTrackingModel>>
    {
        private readonly IReadOnlyRepository<AttendanceTracking> _readAttendanceTrackingRepository;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly GetFullTextFilteredEntitiesSharedQuery _getFullTextFilteredEntitiesSharedQuery;
        private readonly GetUsersSharedQuery _getUsersSharedQuery;

        public GetAttendanceTrackingBySessionIdQueryHandler(
            IReadOnlyRepository<AttendanceTracking> readAttendanceTrackingRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            GetFullTextFilteredEntitiesSharedQuery getFullTextFilteredEntitiesSharedQuery,
            IReadOnlyRepository<CourseUser> readUserRepository,
            GetUsersSharedQuery getUsersSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAttendanceTrackingRepository = readAttendanceTrackingRepository;
            _getFullTextFilteredEntitiesSharedQuery = getFullTextFilteredEntitiesSharedQuery;
            _readUserRepository = readUserRepository;
            _getUsersSharedQuery = getUsersSharedQuery;
        }

        protected override async Task<PagedResultDto<AttendanceTrackingModel>> HandleAsync(GetAttendanceTrackingBySessionIdQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readAttendanceTrackingRepository.GetAll().Where(x => x.SessionId == query.Id);

            IQueryable<CourseUser> filteredUserQuery = null;
            if (!string.IsNullOrEmpty(query.SearchText) || query.Filter?.ContainFilters != null || query.Filter?.FromToFilters != null)
            {
                filteredUserQuery = _getUsersSharedQuery.BySearchText(_readUserRepository.GetAll(), query.SearchText);
                filteredUserQuery = query.Filter?.ContainFilters?.Aggregate(
                filteredUserQuery,
                (current, containFilter) =>
                    current.BuildContainFilter(
                        containFilter.Field,
                        containFilter.Values,
                        NotApplicableMetadataConstants.UserNotApplicableMapping,
                        containFilter.NotContain,
                        (values, fieldName, currentQuery) => _getFullTextFilteredEntitiesSharedQuery.ByFilter(
                            currentQuery,
                            fieldName,
                            values,
                            NotApplicableMetadataConstants.UserNotApplicableMapping.ContainsKey(fieldName) ? NotApplicableMetadataConstants.UserNotApplicableMapping[fieldName] : null))) ?? filteredUserQuery;

                filteredUserQuery = query.Filter?.FromToFilters?.Aggregate(
                    filteredUserQuery,
                    (current, fromToFilter) =>
                        current.BuildFromToFilter(
                            fromToFilter.Field,
                            fromToFilter.FromValue,
                            fromToFilter.ToValue,
                            fromToFilter.EqualFrom,
                            fromToFilter.EqualTo)) ?? filteredUserQuery;
            }

            dbQuery = dbQuery.JoinIf(
                filteredUserQuery != null,
                filteredUserQuery,
                p => p.Userid,
                p => p.Id);

            var totalCount = await dbQuery.CountAsync(cancellationToken);
            dbQuery = dbQuery.OrderByDescending(p => p.ChangedDate ?? p.CreatedDate);
            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            var entities = await dbQuery.Select(x => new AttendanceTrackingModel(x)).ToListAsync(cancellationToken);

            return new PagedResultDto<AttendanceTrackingModel>(totalCount, entities);
        }
    }
}
