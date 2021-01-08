using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class SearchECertificateTemplatesQueryHandler : BaseQueryHandler<SearchECertificateTemplatesQuery, PagedResultDto<ECertificateTemplateModel>>
    {
        private readonly IReadOnlyRepository<ECertificateTemplate> _readEcertificateRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseEntityRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly GetFullTextFilteredEntitiesSharedQuery _getFullTextFilteredEntitiesSharedQuery;

        public SearchECertificateTemplatesQueryHandler(
            IReadOnlyRepository<ECertificateTemplate> readEcertificateRepository,
            IReadOnlyRepository<CourseEntity> readCourseEntityRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            GetFullTextFilteredEntitiesSharedQuery getFullTextFilteredEntitiesSharedQuery,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readEcertificateRepository = readEcertificateRepository;
            _readCourseEntityRepository = readCourseEntityRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _getFullTextFilteredEntitiesSharedQuery = getFullTextFilteredEntitiesSharedQuery;
        }

        protected override async Task<PagedResultDto<ECertificateTemplateModel>> HandleAsync(SearchECertificateTemplatesQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readEcertificateRepository
                .GetAll();

            dbQuery = _getFullTextFilteredEntitiesSharedQuery
                .BySearchText(dbQuery, query.SearchText, p => p.FullTextSearch)
                .OrderBy(p => p.FullTextSearchKey);

            // Apply Access Control
            dbQuery = dbQuery.ApplyAccessControl(AccessControlContext, ECertificateTemplate.HasViewECertificateTemplatePermissionQueryExpr(CurrentUserId));

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            if (query.PageInfo != null && query.PageInfo.MaxResultCount == 0)
            {
                return new PagedResultDto<ECertificateTemplateModel>(totalCount);
            }

            switch (query.SearchType)
            {
                case SearchECertificateType.CourseSelection:
                    {
                        dbQuery = dbQuery.Where(p => p.Status == ECertificateTemplateStatus.Active);

                        var ecertificateResult = await ApplyPaging(dbQuery, query.PageInfo).ToListAsync(cancellationToken);
                        return new PagedResultDto<ECertificateTemplateModel>(
                                totalCount,
                                ecertificateResult.Select(x => new ECertificateTemplateModel(x)).ToList());
                    }

                case SearchECertificateType.CustomECertificateTemplateManagement:
                default:
                    {
                        dbQuery = dbQuery.Where(p => p.IsSystem == false);

                        var items = await LoadWithTotalUsingInfo(dbQuery, query.PageInfo?.SkipCount, query.PageInfo?.MaxResultCount, cancellationToken);

                        return new PagedResultDto<ECertificateTemplateModel>(totalCount, items);
                    }
            }
        }

        private async Task<List<ECertificateTemplateModel>> LoadWithTotalUsingInfo(
            IQueryable<ECertificateTemplate> ecertificateQuery,
            int? skipCount,
            int? take,
            CancellationToken cancellationToken)
        {
            if (skipCount != null)
            {
                ecertificateQuery = ecertificateQuery.Skip(skipCount.Value);
            }

            if (take != null)
            {
                ecertificateQuery = ecertificateQuery.Take(take.Value);
            }

            var ecertificateResult = await ecertificateQuery.ToListAsync(cancellationToken);

            // NOTE: Query eCertificate left join with courseEntity and registration to get totalCoursesUsing and totalLearnersReceived for each eCertificate.
            var ecertificateUsedByCourseQuery = ecertificateQuery
                .GroupJoin(
                    _readCourseEntityRepository.GetAll(),
                    p => p.Id,
                    p => p.ECertificateTemplateId,
                    (ecertificateTemplate, courses) => new { ecertificateTemplateId = ecertificateTemplate.Id, courses })
                .SelectMany(
                    p => p.courses.DefaultIfEmpty(),
                    (gj, course) => new { gj.ecertificateTemplateId, courseId = (Guid?)course.Id });

            var ecertificateReceivedByLearnerQuery = ecertificateUsedByCourseQuery
                .GroupJoin(
                    _readRegistrationRepository.GetAll().Where(Registration.IsParticipantExpr()),
                    p => p.ecertificateTemplateId,
                    p => p.CompleteCourseECertificateId,
                    (gj, registrations) => new { gj.ecertificateTemplateId, gj.courseId, registrations })
                .SelectMany(
                    p => p.registrations.DefaultIfEmpty(),
                    (gj, registration) => new
                    { gj.ecertificateTemplateId, gj.courseId, registrationId = (Guid?)registration.Id });

            var totalUseCertificateInfoQuery = ecertificateReceivedByLearnerQuery
                .GroupBy(p => p.ecertificateTemplateId)
                .Select(p => new
                {
                    p.Key,
                    totalCoursesUsing = p.Select(x => x.courseId).Distinct().Count(),
                    totalLearnersReceived = p.Select(x => x.registrationId).Count()
                });

            var totalUseCertificateInfoDic =
                await totalUseCertificateInfoQuery.ToDictionaryAsync(p => p.Key, cancellationToken);

            var hasRightChecker = _readEcertificateRepository.GetHasAdminRightChecker(ecertificateResult, AccessControlContext);

            return ecertificateResult.Select(x =>
            {
                var totalUseCertificateInfo = totalUseCertificateInfoDic.ContainsKey(x.Id) ? totalUseCertificateInfoDic[x.Id] : null;
                return new ECertificateTemplateModel(
                        x,
                        totalUseCertificateInfo != null ? totalUseCertificateInfo.totalCoursesUsing : 0,
                        totalUseCertificateInfo != null ? totalUseCertificateInfo.totalLearnersReceived : 0,
                        hasRightChecker(x));
            }).ToList();
        }
    }
}
