using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.DomainExtensions;
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
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class SearchCoursesQueryHandler : BaseQueryHandler<SearchCoursesQuery, PagedResultDto<CourseModel>>
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly GetFullTextFilteredEntitiesSharedQuery _getFullTextFilteredEntitiesSharedQuery;
        private readonly CheckCourseHasContentLogic _checkCourseHasContentLogic;

        public SearchCoursesQueryHandler(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            GetFullTextFilteredEntitiesSharedQuery getFullTextFilteredEntitiesSharedQuery,
            CheckCourseHasContentLogic checkCourseHasContentLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
            _readCourseRepository = readCourseRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _getFullTextFilteredEntitiesSharedQuery = getFullTextFilteredEntitiesSharedQuery;
            _checkCourseHasContentLogic = checkCourseHasContentLogic;
        }

        protected override async Task<PagedResultDto<CourseModel>> HandleAsync(SearchCoursesQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readCourseRepository.GetAll();

            dbQuery = dbQuery.WhereIf(query.CoursePlanningCycleId != null, p => p.CoursePlanningCycleId == query.CoursePlanningCycleId);

            // Full search text need to be put at the first to prevent CONTAINS SQL need to be applied directly into Course table, not Join query
            dbQuery = _getFullTextFilteredEntitiesSharedQuery.BySearchText(dbQuery, query.SearchText, course => course.FullTextSearch);

            dbQuery = query.Filter?.ContainFilters?.Aggregate(
                dbQuery,
                (current, containFilter) =>
                    current.BuildContainFilter(
                        containFilter.Field,
                        containFilter.Values,
                        NotApplicableMetadataConstants.CourseNotApplicableMapping,
                        containFilter.NotContain,
                        (values, fieldName, currentQuery) => _getFullTextFilteredEntitiesSharedQuery.ByFilter(
                            currentQuery,
                            fieldName,
                            values,
                            NotApplicableMetadataConstants.CourseNotApplicableMapping.ContainsKey(fieldName) ? NotApplicableMetadataConstants.CourseNotApplicableMapping[fieldName] : null))) ?? dbQuery;

            dbQuery = query.Filter?.FromToFilters?.Aggregate(
                dbQuery,
                (current, fromToFilter) =>
                    current.BuildFromToFilter(
                        fromToFilter.Field,
                        fromToFilter.FromValue,
                        fromToFilter.ToValue,
                        fromToFilter.EqualFrom,
                        fromToFilter.EqualTo)) ?? dbQuery;

            dbQuery = FilterBySearchType(dbQuery, query.SearchType);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            if (query.PageInfo != null && query.PageInfo.MaxResultCount == 0)
            {
                return new PagedResultDto<CourseModel>(totalCount);
            }

            dbQuery = dbQuery.OrderByDescending(p => p.FullTextSearchKey);

            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            var entities = await CheckCourseHasContentAsync(dbQuery.ToList(), query.CheckCourseContent, cancellationToken);

            return new PagedResultDto<CourseModel>(totalCount, entities);
        }

        private IQueryable<CourseEntity> FilterBySearchType(
            IQueryable<CourseEntity> dbQuery,
            SearchCourseType searchType)
        {
            switch (searchType)
            {
                case SearchCourseType.Approver:
                    {
                        return dbQuery
                            .ApplyAccessControl(AccessControlContext, CourseEntity.HasApprovalPermissionExpr(CurrentUserId, CurrentUserRoles))
                            .Where(x => x.Status == CourseStatus.PendingApproval)
                            .Where(CourseEntity.IsNotArchivedExpr());
                    }

                case SearchCourseType.ClassApprover:
                    {
                        var pendingClassRunCourseIds = _readClassRunRepository.GetAll()
                                .Where(x => (x.CancellationStatus != null && x.CancellationStatus == ClassRunCancellationStatus.PendingApproval)
                                            || (x.RescheduleStatus != null && x.RescheduleStatus == ClassRunRescheduleStatus.PendingApproval))
                                .Select(x => x.CourseId);

                        var pendingClassRunCourse = from x in dbQuery
                                                    join y in pendingClassRunCourseIds on x.Id equals y
                                                    select x;
                        return pendingClassRunCourse.Distinct()
                            .ApplyAccessControl(AccessControlContext, CourseEntity.HasApprovalPermissionExpr(CurrentUserId, CurrentUserRoles))
                            .Where(CourseEntity.IsNotArchivedExpr());
                    }

                case SearchCourseType.RegistrationApprover:
                    {
                        var pendingRegistrations = _readRegistrationRepository.GetAll()
                                .Where(Registration.IsPendingConfirmationExpr().Or(Registration.IsPendingWithdrawalConfirmExpr()))
                                .Select(x => new { x.CourseId, x.ClassRunId });

                        var classRunNotStartedIds = _readClassRunRepository.GetAll()
                                .Where(ClassRun.StartedExpr().Not())
                                .Select(x => x.Id);

                        var pendingRegistrationCourses = from x in dbQuery
                                                         join y in pendingRegistrations on x.Id equals y.CourseId
                                                         join z in classRunNotStartedIds on y.ClassRunId equals z
                                                         select x;
                        return pendingRegistrationCourses.Distinct()
                            .ApplyAccessControl(AccessControlContext, CourseEntity.HasAdministrationPermissionExpr(CurrentUserId, CurrentUserRoles))
                            .Where(CourseEntity.IsNotArchivedExpr());
                    }

                case SearchCourseType.ContentApprover:
                    {
                        var pendingClassRunCourseIds = _readClassRunRepository
                            .GetAll()
                            .Where(x => x.ContentStatus == ContentStatus.PendingApproval)
                            .Select(x => x.CourseId);

                        var pendingClassRunCourse = from x in dbQuery.ApplyAccessControl(AccessControlContext, CourseEntity.HasApprovalPermissionExpr(CurrentUserId, CurrentUserRoles))
                                                    join y in pendingClassRunCourseIds on x.Id equals y
                                                    select x;

                        return dbQuery.ApplyAccessControl(AccessControlContext, CourseEntity.HasApprovalPermissionExpr(CurrentUserId, CurrentUserRoles))
                                      .Where(p => p.ContentStatus == ContentStatus.PendingApproval)
                                      .Where(CourseEntity.AfterApprovingExpr())
                                      .Where(CourseEntity.IsNotArchivedExpr())
                                      .Union(pendingClassRunCourse)
                                      .Distinct();
                    }

                case SearchCourseType.LearningManagement:
                    {
                        return dbQuery.ApplyAccessControl(AccessControlContext, CourseEntityExtensions.HasViewContentPermissionQueryExpr(CurrentUserId, CurrentUserRoles))
                            .Where(CourseEntity.AfterApprovingExpr())
                            .Where(CourseEntity.IsNotArchivedExpr());
                    }

                case SearchCourseType.Recurring:
                    {
                        return dbQuery
                            .Where(CourseEntity.IsMicroLearningExpr().Not())
                            .ApplyAccessControl(AccessControlContext, CourseEntityExtensions.HasContentCreatorPermissionQueryExpr(CurrentUserId, CurrentUserRoles))
                            .Where(x => x.Status == CourseStatus.Completed)
                            .Where(CourseEntity.IsNotArchivedExpr());
                    }

                case SearchCourseType.Prerequisite:
                    {
                        return dbQuery
                            .Where(CourseEntity.IsMicroLearningExpr().Not())
                            .Where(p => p.Status == CourseStatus.Published || p.Status == CourseStatus.Unpublished)
                            .Where(CourseEntity.IsNotArchivedExpr());
                    }

                case SearchCourseType.Learner:
                    {
                        return dbQuery
                            .Where(p => p.Status == CourseStatus.Published)
                            .Where(CourseEntity.IsNotArchivedExpr());
                    }

                case SearchCourseType.CourseInCoursePlanningCycle:
                    {
                        return UserContext.IsInRole(UserRoles.CoursePlanningCoordinator)
                            ? dbQuery
                            : dbQuery
                                .ApplyAccessControl(
                                    AccessControlContext,
                                    CourseEntityExtensions.HasViewCourseOrClassRunPermissionQueryExpr(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)))
                                .Where(CourseEntity.IsNotArchivedExpr());
                    }

                case SearchCourseType.ApproverCoursePlanning:
                    {
                        return dbQuery
                           .ApplyAccessControl(AccessControlContext, CourseEntity.HasApprovalPermissionExpr(CurrentUserId, CurrentUserRoles))
                           .Where(x => x.Status == CourseStatus.PendingApproval)
                           .Where(CourseEntity.IsNotArchivedExpr());
                    }

                case SearchCourseType.VerifyCoursePlanning:
                    {
                        return dbQuery
                            .Where(x => CoursePlanningCycle.HasVerificationPermission(CurrentUserId, CurrentUserRoles))
                            .Where(x => x.Status == CourseStatus.Approved)
                            .Where(CourseEntity.IsNotArchivedExpr());
                    }

                case SearchCourseType.CopyMetadataRight:
                    {
                        return dbQuery.ApplyAccessControl(AccessControlContext, CourseEntityExtensions.CanCopyDataQueryExpr(CurrentUserId, CurrentUserRoles));
                    }

                case SearchCourseType.Archived:
                    {
                        return dbQuery
                            .ApplyAccessControl(AccessControlContext, CourseEntityExtensions.CanCopyDataQueryExpr(CurrentUserId, CurrentUserRoles))
                            .Where(CourseEntity.IsNotArchivedExpr().Not());
                    }

                case SearchCourseType.Cloneable:
                    {
                        return dbQuery
                            .ApplyAccessControl(AccessControlContext, CourseEntityExtensions.HasViewCourseOrClassRunPermissionQueryExpr(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)))
                            .Where(CourseEntity.CanCloneValidator().IsValidExpression);
                    }

                case SearchCourseType.Owner:
                default:
                    {
                        return dbQuery
                            .ApplyAccessControl(AccessControlContext, CourseEntityExtensions.HasViewCourseOrClassRunPermissionQueryExpr(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)))
                            .Where(CourseEntity.IsNotArchivedExpr());
                    }
            }
        }

        private async Task<List<CourseModel>> CheckCourseHasContentAsync(List<CourseEntity> entities, bool checkCourseContent, CancellationToken cancellationToken)
        {
            var hasRightChecker = _readCourseRepository.GetHasAdminRightChecker(entities, AccessControlContext);

            if (checkCourseContent)
            {
                var courseHasContentDic = await _checkCourseHasContentLogic.ByCourseIds(entities.Select(p => p.Id).ToList(), cancellationToken);
                return entities.Select(p => new CourseModel(p, hasRightChecker(p), courseHasContentDic[p.Id])).ToList();
            }

            return entities.Select(p => new CourseModel(p, hasRightChecker(p), null)).ToList();
        }
    }
}
