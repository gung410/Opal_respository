using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Content.Application.Models;
using Microservice.Content.Domain.Entities;
using Microservice.Content.Domain.Enums;
using Microservice.Content.Infrastructure;
using Microservice.Content.Versioning.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Content.Application.Queries.QueryHandlers
{
    public class GetPendingApprovalDigitalContentsQueryHandler : BaseQueryHandler<GetPendingApprovalDigitalContentsQuery, PagedResultDto<SearchDigitalContentModel>>
    {
        private readonly IRepository<DigitalContent> _dcRepository;

        public GetPendingApprovalDigitalContentsQueryHandler(
            IRepository<DigitalContent> dcRepository,
            IAccessControlContext accessControlContext) : base(accessControlContext)
        {
            _dcRepository = dcRepository;
        }

        protected override async Task<PagedResultDto<SearchDigitalContentModel>> HandleAsync(GetPendingApprovalDigitalContentsQuery query, CancellationToken cancellationToken)
        {
            IQueryable<DigitalContent> dbQuery = _dcRepository
                .GetAllWithAccessControl(AccessControlContext, DigitalContentExpressions.HasApprovalPermissionExpr(CurrentUserId))
                .Where(p => p.Status == DigitalContentStatus.PendingForApproval)
                .IgnoreArchivedItems();

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderByDescending(p => p.ChangedDate != null ? p.ChangedDate : p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.PagedInfo);

            var entities = await dbQuery.Select(p => MappingToDigitalContentModel(p)).ToListAsync(cancellationToken);

            return new PagedResultDto<SearchDigitalContentModel>(totalCount, entities);
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
                ArchiveDate = dc.ArchiveDate,
                ArchivedBy = dc.ArchivedBy
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
