using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Application.Models;
using Microservice.Content.Application.RequestDtos;
using Microservice.Content.Common.Extensions;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;
using Microservice.Content.Infrastructure;
using Microservice.Content.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Helpers;
using Thunder.Platform.Core.Timing;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class SearchDigitalContentQueryHandler : BaseQueryHandler<SearchDigitalContentQuery, PagedResultDto<SearchDigitalContentModel>>
    {
        private readonly IRepository<DigitalContent> _dcRepository;
        private readonly IRepository<UploadedContent> _ucRepository;
        private readonly IRepository<AccessRight> _accessRightRepository;

        public SearchDigitalContentQueryHandler(
            IAccessControlContext accessControlContext,
            IRepository<AccessRight> accessRightRepository,
            IRepository<DigitalContent> dcRepository,
            IRepository<UploadedContent> ucRepository) : base(accessControlContext)
        {
            _dcRepository = dcRepository;
            _ucRepository = ucRepository;
            _accessRightRepository = accessRightRepository;
        }

        protected override async Task<PagedResultDto<SearchDigitalContentModel>> HandleAsync(SearchDigitalContentQuery query, CancellationToken cancellationToken)
        {
            var fiveWorkingDayAgo = DateTimeHelper.GetDayBeforeXWorkingDayFromNow(5);
            var unionContentExpression = query.Request.IncludeContentForImportToCourse == true
                ? DigitalContentExpressions.AvailableForImportToCourseExpr(query.UserId)
                : DigitalContentExpressions.HasOwnerOrApprovalPermissionExpr(query.UserId);

            IQueryable<DigitalContent> dbQuery = _dcRepository
                .GetAllWithAccessControl(AccessControlContext, unionContentExpression)
                .CombineWithAccessRight(_dcRepository, _accessRightRepository, CurrentUserId)
                .IgnoreArchivedItems();

            switch (query.Request.QueryMode)
            {
                case DigitalContentQueryMode.PendingApproval:
                    dbQuery = dbQuery.Where(r =>
                    (r.AlternativeApprovingOfficerId == query.UserId) ?
                        (r.Status == DigitalContentStatus.PendingForApproval && r.SubmitDate <= fiveWorkingDayAgo)
                      : (r.Status == DigitalContentStatus.PendingForApproval));
                    break;
                case DigitalContentQueryMode.Approved:
                    dbQuery = dbQuery.Where(r =>
                        r.Status == DigitalContentStatus.Approved
                      || r.Status == DigitalContentStatus.Published
                      || r.Status == DigitalContentStatus.Unpublished);
                    break;
                case DigitalContentQueryMode.Archived:
                    dbQuery = dbQuery.Where(r => r.Status == DigitalContentStatus.Archived);
                    break;
                default:
                    dbQuery = dbQuery.Where(r =>
                        (r.Status == DigitalContentStatus.PendingForApproval &&
                         r.AlternativeApprovingOfficerId == query.UserId) ?
                        (r.SubmitDate <= fiveWorkingDayAgo) : r.Status != DigitalContentStatus.Archived);
                    break;
            }

            dbQuery = dbQuery.WhereIf(
                query.Request.WithinCopyrightDuration == true,
                _ => (_.StartDate == null || EF.Functions.DateDiffMinute(Clock.Now, _.StartDate.Value) <= 0)
                && (_.ExpiredDate == null || EF.Functions.DateDiffMinute(Clock.Now, _.ExpiredDate.Value) >= 0));

            dbQuery = dbQuery.WhereIf(
                !string.IsNullOrEmpty(query.Request.SearchText),
                r => r.Title.Contains(query.Request.SearchText)
                     || r.Description.Contains(query.Request.SearchText));

            dbQuery = query.Request.FilterByStatus.Contains(DigitalContentStatus.Expired)
                ? dbQuery.Where(digitalContent => digitalContent.ExpiredDate.HasValue
                    && (EF.Functions.DateDiffMinute(Clock.Now, digitalContent.ExpiredDate.Value) <= 0))
                : dbQuery.WhereIf(query.Request.FilterByStatus.Any(), digitalContent => query.Request.FilterByStatus.Contains(digitalContent.Status));

            // Filter downloadable contents
            dbQuery = dbQuery.WhereIf(query.Request.WithinDownloadableContent.HasValue, m => m.IsAllowDownload == query.Request.WithinDownloadableContent.Value);

            // Filter contents by extensions.
            if (query.Request.FilterByExtensions != null)
            {
                IQueryable<DigitalContent> extQuery = _ucRepository
                    .GetAll()
                    .Where(_ => query.Request.FilterByExtensions.Any(n => n == _.FileExtension));
                dbQuery = dbQuery.Where(p => extQuery.Any(n => n.Id == p.Id));
            }

            if (query.Request.FilterByStatus.Contains(DigitalContentStatus.PendingForApproval))
            {
                dbQuery = dbQuery.Where(r => (r.AlternativeApprovingOfficerId == query.UserId) ?
                        (r.Status == DigitalContentStatus.PendingForApproval && r.SubmitDate <= fiveWorkingDayAgo)
                      : (r.Status == DigitalContentStatus.PendingForApproval));
            }

            var totalCount = dbQuery.Count();

            Expression<Func<DigitalContent, object>> firstSortExpression = p => EF.Property<object>(p, query.Request.SortField);
            Expression<Func<DigitalContent, object>> secondSortExpression = p => p.ChangedDate;

            bool useSecondSortExpression = query.Request.SortField.ToLower() != "ChangedDate".ToLower();

            var dbQueryWithOrder = query.Request.SortDirection == SortDirection.Ascending ? dbQuery.OrderBy(firstSortExpression) : dbQuery.OrderByDescending(firstSortExpression);
            dbQuery = useSecondSortExpression ? dbQueryWithOrder.ThenByDescending(secondSortExpression) : dbQueryWithOrder;

            dbQuery = ApplyPaging(dbQuery, query.Request.PagedInfo);

            var entities = dbQuery.Select(p => MappingToDigitalContentModel(p)).ToList();

            return await Task.FromResult(new PagedResultDto<SearchDigitalContentModel>(totalCount, entities));
        }

        private static SearchDigitalContentModel MappingToDigitalContentModel(DigitalContent dc)
        {
            var dcModel = new SearchDigitalContentModel
            {
                Id = dc.Id,
                Title = dc.Title,
                Status = dc.Status,
                Type = dc.Type,
                Description = HttpUtility.HtmlDecode(dc.Description),
                OwnerId = dc.OwnerId,
                ExternalId = dc.ExternalId,
                CreatedDate = dc.CreatedDate,
                ChangedDate = dc.ChangedDate ?? dc.CreatedDate,
                ExpiredDate = dc.ExpiredDate,
                Publisher = dc.Publisher,
                Source = dc.Source,
                Copyright = dc.Copyright,
                TermsOfUse = dc.TermsOfUse,
                ArchivedBy = dc.ArchivedBy,
                ArchiveDate = dc.ArchiveDate,
                IsAllowDownload = dc.IsAllowDownload
            };

            if (dc is UploadedContent uc)
            {
                dcModel.FileExtension = uc.FileExtension;
                dcModel.FileDuration = uc.FileDuration;
                dcModel.FileType = uc.FileType;
            }

            return dcModel;
        }
    }
}
