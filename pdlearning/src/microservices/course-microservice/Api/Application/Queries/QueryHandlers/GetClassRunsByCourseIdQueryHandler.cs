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
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetClassRunsByCourseIdQueryHandler : BaseQueryHandler<GetClassRunsByCourseIdQuery, PagedResultDto<ClassRunModel>>
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly CheckClassRunHasContentLogic _checkClassRunHasContentLogic;

        public GetClassRunsByCourseIdQueryHandler(
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            CheckClassRunHasContentLogic checkClassRunHasContentLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
            _readCourseRepository = readCourseRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _checkClassRunHasContentLogic = checkClassRunHasContentLogic;
        }

        protected override async Task<PagedResultDto<ClassRunModel>> HandleAsync(
            GetClassRunsByCourseIdQuery query,
            CancellationToken cancellationToken)
        {
            var dbQuery = _readClassRunRepository.GetAll().Where(x => x.CourseId == query.CourseId);
            dbQuery = dbQuery.WhereIf(!string.IsNullOrEmpty(query.SearchText), p => EF.Functions.Like(p.ClassTitle, $"%{query.SearchText}%") || query.SearchText == p.ClassRunCode);
            dbQuery = dbQuery.WhereIf(query.NotStarted == true, ClassRun.NotStartedExpr());
            dbQuery = dbQuery.WhereIf(query.NotStarted == false && query.NotEnded == true, ClassRun.NotEndedExpr());
            dbQuery = query.Filter?.ContainFilters?.Aggregate(
                dbQuery,
                (current, containFilter) =>
                    current.BuildContainFilter(
                        containFilter.Field,
                        containFilter.Values,
                        null,
                        containFilter.NotContain,
                        (values, fieldType, currentQuery) => currentQuery)) ?? dbQuery;

            dbQuery = query.Filter?.FromToFilters?.Aggregate(
                dbQuery,
                (current, fromToFilter) =>
                    current.BuildFromToFilter(
                        fromToFilter.Field,
                        fromToFilter.FromValue,
                        fromToFilter.ToValue,
                        fromToFilter.EqualFrom,
                        fromToFilter.EqualTo)) ?? dbQuery;

            dbQuery = await FilterBySearchType(dbQuery, query.SearchType, query.CourseId, cancellationToken);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            if (query.PageInfo != null && query.PageInfo.MaxResultCount == 0)
            {
                return new PagedResultDto<ClassRunModel>(totalCount);
            }

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            var entities = await dbQuery.Select(x => new ClassRunModel(x)).ToListAsync(cancellationToken);

            entities = await LoadClassRunHasContentInfoAsync(entities, query.LoadHasContentInfo, cancellationToken);

            return new PagedResultDto<ClassRunModel>(totalCount, entities);
        }

        private async Task<List<ClassRunModel>> LoadClassRunHasContentInfoAsync(List<ClassRunModel> entities, bool? loadHasContentInfo, CancellationToken cancellationToken)
        {
            if (loadHasContentInfo == true)
            {
                var classRunHasContentDic = await _checkClassRunHasContentLogic
                    .ByClassRunIds(entities.Select(p => p.Id).ToList(), cancellationToken);
                return entities.Select(p => p.SetHasContent(classRunHasContentDic[p.Id])).ToList();
            }

            return entities;
        }

        private async Task<IQueryable<ClassRun>> FilterBySearchType(
            IQueryable<ClassRun> dbQuery,
            SearchClassRunType searchType,
            Guid courseId,
            CancellationToken cancellationToken)
        {
            switch (searchType)
            {
                case SearchClassRunType.OrganisationDevelopment:
                    {
                        // Show class runs can be nominated: Class run must be published and not started. If course is E-Learning and not ended, class runs can be nominated.
                        var course = await _readCourseRepository.GetAsync(courseId);
                        return dbQuery
                            .Where(x => x.Status == ClassRunStatus.Published)
                            .Where(ClassRun.NotStartedExpr().Or(ClassRun.NotEndedAndIsELearningExpr(course)));
                    }

                case SearchClassRunType.Learner:
                    {
                        var learnerClassRunIds = await _readRegistrationRepository
                            .GetAll()
                            .Where(p => p.UserId == CurrentUserId)
                            .Where(p => p.CourseId == courseId)
                            .Select(p => p.ClassRunId)
                            .ToListAsync(cancellationToken);
                        return dbQuery.Where(x => x.Status == ClassRunStatus.Published || learnerClassRunIds.Contains(x.Id));
                    }

                case SearchClassRunType.CancellationPending:
                    {
                        EnsureValidPermission(await HasValidApprovalPermission(courseId, cancellationToken));

                        return dbQuery.Where(x => x.CancellationStatus.HasValue && x.CancellationStatus.Value == ClassRunCancellationStatus.PendingApproval);
                    }

                case SearchClassRunType.NotApprovedCancellation:
                    {
                        EnsureValidPermission(await HasValidApprovalPermission(courseId, cancellationToken));

                        return dbQuery.Where(x => x.CancellationStatus.HasValue
                                                 && (x.CancellationStatus.Value == ClassRunCancellationStatus.PendingApproval
                                                 || x.CancellationStatus.Value == ClassRunCancellationStatus.Rejected));
                    }

                case SearchClassRunType.LearningManagement:
                    {
                        var hasCoursePermission = await _readCourseRepository
                            .GetAllWithAccessControl(AccessControlContext, CourseEntityExtensions.HasViewContentPermissionQueryExpr(CurrentUserId, CurrentUserRoles))
                            .Where(x => x.Id == courseId)
                            .AnyAsync(cancellationToken);

                        EnsureValidPermission(hasCoursePermission);

                        return dbQuery;
                    }

                case SearchClassRunType.ReschedulePending:
                    {
                        EnsureValidPermission(await HasValidApprovalPermission(courseId, cancellationToken));

                        return dbQuery.Where(x => x.RescheduleStatus.HasValue && x.RescheduleStatus.Value == ClassRunRescheduleStatus.PendingApproval);
                    }

                case SearchClassRunType.AllReschedule:
                    {
                        EnsureValidPermission(await HasValidApprovalPermission(courseId, cancellationToken));

                        return dbQuery.Where(x => x.RescheduleStatus.HasValue);
                    }

                case SearchClassRunType.RegistrationApprover:
                    {
                        var hasCoursePermission = await _readCourseRepository
                            .GetAllWithAccessControl(AccessControlContext, CourseEntity.HasAdministrationPermissionExpr(CurrentUserId, CurrentUserRoles))
                            .Where(x => x.Id == courseId)
                            .AnyAsync(cancellationToken);

                        EnsureValidPermission(hasCoursePermission);

                        var pendingRegistrationClassrunIds = _readRegistrationRepository.GetAll()
                                .Where(Registration.IsPendingConfirmationExpr().Or(Registration.IsPendingWithdrawalConfirmExpr()))
                                .Select(x => x.ClassRunId);

                        dbQuery = dbQuery.Where(ClassRun.StartedExpr().Not());

                        var pendingRegistrationClassruns = from x in dbQuery
                                                           join y in pendingRegistrationClassrunIds on x.Id equals y
                                                           select x;

                        return pendingRegistrationClassruns.Distinct();
                    }

                default:
                    {
                        var hasCoursePermission = await _readCourseRepository
                            .GetAllWithAccessControl(AccessControlContext, CourseEntityExtensions.HasViewCourseOrClassRunPermissionQueryExpr(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)))
                            .Where(x => x.Id == courseId)
                            .AnyAsync(cancellationToken);

                        EnsureValidPermission(hasCoursePermission);

                        return dbQuery;
                    }
            }
        }

        private async Task<bool> HasValidApprovalPermission(Guid courseId, CancellationToken cancellationToken)
        {
            var hasCoursePermission = await _readCourseRepository
                .GetAllWithAccessControl(AccessControlContext, CourseEntity.HasApprovalPermissionExpr(CurrentUserId, CurrentUserRoles))
                .Where(x => x.Id == courseId)
                .AnyAsync(cancellationToken);
            return hasCoursePermission;
        }
    }
}
