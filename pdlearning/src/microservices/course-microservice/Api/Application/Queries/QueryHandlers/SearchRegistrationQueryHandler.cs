using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class SearchRegistrationQueryHandler : BaseQueryHandler<SearchRegistrationQuery, PagedResultDto<RegistrationModel>>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<ParticipantAssignmentTrack> _readParticipantAssignmentTrackRepository;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;
        private readonly GetFullTextFilteredEntitiesSharedQuery _getFullTextFilteredEntitiesSharedQuery;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;

        public SearchRegistrationQueryHandler(
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            GetFullTextFilteredEntitiesSharedQuery getFullTextFilteredEntitiesSharedQuery,
            IReadOnlyRepository<CourseUser> readUserRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readParticipantAssignmentTrackRepository = readParticipantAssignmentTrackRepository;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _getFullTextFilteredEntitiesSharedQuery = getFullTextFilteredEntitiesSharedQuery;
            _readUserRepository = readUserRepository;
            _readCourseRepository = readCourseRepository;
        }

        protected override async Task<PagedResultDto<RegistrationModel>> HandleAsync(SearchRegistrationQuery query, CancellationToken cancellationToken)
        {
            var registrationQuery = _readRegistrationRepository.GetAll()
                .WhereIf(query.CourseId != null, x => x.CourseId == query.CourseId)
                .WhereIf(query.ClassRunIds != null && query.ClassRunIds.Any(), p => query.ClassRunIds.Contains(p.ClassRunId));

            registrationQuery = ApplyUserFilter(query, registrationQuery);
            registrationQuery = ApplySearchTextFilter(query, registrationQuery);
            registrationQuery = ApplyFilter(query, registrationQuery);

            switch (query.SearchType)
            {
                case SearchRegistrationType.ClassRunRegistration:
                    {
                        registrationQuery = registrationQuery.Where(Registration.IsClassRunRegistrationExpr());
                        break;
                    }

                case SearchRegistrationType.Participant:
                    {
                        var participantRegistrationQuery = registrationQuery.Where(Registration.IsParticipantExpr());
                        if (query.ExcludeAssignedAssignmentId == null)
                        {
                            registrationQuery = participantRegistrationQuery;
                        }
                        else
                        {
                            var participantExcludeAssignmentTrackIds = _readParticipantAssignmentTrackRepository.GetAll()
                                .Join(participantRegistrationQuery, p => p.RegistrationId, p => p.Id, (participant, registration) => participant)
                                .Where(p => p.AssignmentId == query.ExcludeAssignedAssignmentId)
                                .Select(p => p.RegistrationId)
                                .Distinct()
                                .ToList();
                            registrationQuery = participantRegistrationQuery.Where(p => !participantExcludeAssignmentTrackIds.Contains(p.Id));
                        }

                        break;
                    }

                case SearchRegistrationType.Waitlist:
                    {
                        registrationQuery = registrationQuery.Where(Registration.IsWaitlistExpr());
                        break;
                    }

                case SearchRegistrationType.Withdrawal:
                    {
                        registrationQuery = registrationQuery.Where(Registration.IsWithdrawalAdministratingExpr());
                        break;
                    }

                case SearchRegistrationType.ChangeClassRun:
                    {
                        registrationQuery = registrationQuery.Where(Registration.IsChangeClassExpr());
                        break;
                    }

                case SearchRegistrationType.CurrentUser:
                    {
                        registrationQuery = registrationQuery.Where(x => x.UserId == CurrentUserId);
                        break;
                    }

                case SearchRegistrationType.AddedByCA:
                    {
                        registrationQuery = registrationQuery.Where(x => x.RegistrationType == RegistrationType.AddedByCA);
                        break;
                    }

                case SearchRegistrationType.IssuanceTracking:
                    {
                        registrationQuery = registrationQuery.Where(x => x.ECertificate != null);
                        break;
                    }
            }

            var totalCount = await registrationQuery.CountAsync(cancellationToken);

            if (query.PageInfo != null && query.PageInfo.MaxResultCount == 0)
            {
                return new PagedResultDto<RegistrationModel>(totalCount);
            }

            registrationQuery = registrationQuery.OrderByDescending(p => query.SearchType == SearchRegistrationType.IssuanceTracking ? p.LearningCompletedDate : (p.RegistrationDate ?? p.CreatedDate));
            registrationQuery = ApplyPaging(registrationQuery, query.PageInfo);

            if (query.IncludeUserInfo && !query.IncludeCourseClassRun)
            {
                var aggregatedRegistrations = await _getAggregatedRegistrationSharedQuery.WithUserByRegistrationsQuery(registrationQuery, cancellationToken);
                return new PagedResultDto<RegistrationModel>(totalCount, aggregatedRegistrations.Select(p => new RegistrationModel(p.Registration, p.User)).ToList());
            }

            if (!query.IncludeUserInfo && query.IncludeCourseClassRun)
            {
                var aggregatedRegistrations = await _getAggregatedRegistrationSharedQuery.WithClassAndCourseByRegistrationQuery(registrationQuery, cancellationToken);
                return new PagedResultDto<RegistrationModel>(
                    totalCount,
                    aggregatedRegistrations.Select(p => new RegistrationModel(p.Registration, null, p.Course, p.ClassRun)).ToList());
            }

            if (query.IncludeUserInfo && query.IncludeCourseClassRun)
            {
                var aggregatedRegistrations = await _getAggregatedRegistrationSharedQuery.FullByQuery(registrationQuery, null, cancellationToken);
                return new PagedResultDto<RegistrationModel>(
                    totalCount,
                    aggregatedRegistrations.Select(p => new RegistrationModel(p.Registration, p.User, p.Course, p.ClassRun)).ToList());
            }
            else
            {
                var registrations = await registrationQuery.ToListAsync(cancellationToken);
                return new PagedResultDto<RegistrationModel>(totalCount, registrations.Select(p => new RegistrationModel(p)).ToList());
            }
        }

        private static IQueryable<Registration> ApplyFilter(SearchRegistrationQuery query, IQueryable<Registration> registrationQuery)
        {
            registrationQuery = query.Filter?.ContainFilters?.Aggregate(
                        registrationQuery,
                        (current, containFilter) =>
                            current.BuildContainFilter(
                                containFilter.Field,
                                containFilter.Values,
                                null,
                                containFilter.NotContain)) ?? registrationQuery;

            registrationQuery = query.Filter?.FromToFilters?.Aggregate(
                registrationQuery,
                (current, fromToFilter) =>
                    current.BuildFromToFilter(
                        fromToFilter.Field,
                        fromToFilter.FromValue,
                        fromToFilter.ToValue,
                        fromToFilter.EqualFrom,
                        fromToFilter.EqualTo)) ?? registrationQuery;
            return registrationQuery;
        }

        private IQueryable<Registration> ApplySearchTextFilter(SearchRegistrationQuery query, IQueryable<Registration> registrationQuery)
        {
            if (!string.IsNullOrEmpty(query.SearchText))
            {
                var searchedUsersQuery = _getFullTextFilteredEntitiesSharedQuery.BySearchText(_readUserRepository.GetAll(), query.SearchText, p => p.FullTextSearch);
                var filteredRegistrationBySearchUserQuery = registrationQuery.Join(searchedUsersQuery, p => p.UserId, p => p.Id, (registration, searchedUser) => registration);

                if (query.ApplySearchTextForCourse)
                {
                    var searchedCoursesQuery = _getFullTextFilteredEntitiesSharedQuery.BySearchText(_readCourseRepository.GetAll(), query.SearchText, p => p.FullTextSearch);
                    var filteredRegistrationBySearchCourseQuery = registrationQuery.Join(searchedCoursesQuery, p => p.CourseId, p => p.Id, (registration, searchedCourse) => registration);
                    return filteredRegistrationBySearchUserQuery.Union(filteredRegistrationBySearchCourseQuery).Distinct();
                }

                return filteredRegistrationBySearchUserQuery;

                // TODO: This is Solution 2. Keep it here to check in SPT which is better.
                // registrationQuery = registrationQuery
                //    .GroupJoin(searchedUsersQuery, p => p.UserId, p => p.Id, (registration, searchedUsers) => new { registration, searchedUsers })
                //    .SelectMany(p => p.searchedUsers.DefaultIfEmpty(), (gj, searchedUser) => new { gj.registration, searchedUser })
                //    .GroupJoin(searchedCoursesQuery, p => p.registration.CourseId, p => p.Id, (gj, searchedCourses) => new { gj.registration, gj.searchedUser, searchedCourses })
                //    .SelectMany(p => p.searchedCourses.DefaultIfEmpty(), (gj, searchedCourse) => new { gj.registration, searchedUserId = (Guid?)gj.searchedUser.Id, searchedCourseId = (Guid?)searchedCourse.Id })
                //    .Where(p => p.searchedUserId != null || p.searchedCourseId != null)
                //    .Select(p => p.registration);
            }

            return registrationQuery;
        }

        private IQueryable<Registration> ApplyUserFilter(SearchRegistrationQuery query, IQueryable<Registration> registrationQuery)
        {
            IQueryable<CourseUser> filteredUserQuery = query.HasUserFilter() ? _readUserRepository.GetAll() : null;

            filteredUserQuery = query.UserFilter?.ContainFilters?.Aggregate(
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

            filteredUserQuery = query.UserFilter?.FromToFilters?.Aggregate(
                filteredUserQuery,
                (current, fromToFilter) =>
                    current.BuildFromToFilter(
                        fromToFilter.Field,
                        fromToFilter.FromValue,
                        fromToFilter.ToValue,
                        fromToFilter.EqualFrom,
                        fromToFilter.EqualTo)) ?? filteredUserQuery;

            registrationQuery =
                registrationQuery.JoinIf(
                    filteredUserQuery != null,
                    filteredUserQuery,
                    p => p.UserId,
                    p => p.Id);

            return registrationQuery;
        }
    }
}
