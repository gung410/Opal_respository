using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.BrokenLink.Application.Models;
using Microservice.BrokenLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.BrokenLink.Application.Queries.QueryHandlers
{
    public class SearchBrokenLinkReportQueryHandler : BaseThunderQueryHandler<SearchBrokenLinkReportQuery, PagedResultDto<BrokenLinkReportModel>>
    {
        private readonly IRepository<BrokenLinkReport> _brokenLinkReportRepository;

        public SearchBrokenLinkReportQueryHandler(IRepository<BrokenLinkReport> brokenLinkReportRepository)
        {
            this._brokenLinkReportRepository = brokenLinkReportRepository;
        }

        protected override async Task<PagedResultDto<BrokenLinkReportModel>> HandleAsync(SearchBrokenLinkReportQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _brokenLinkReportRepository
                .GetAll();

            dbQuery = dbQuery.WhereIf(query.Request.OriginalObjectId.HasValue, p => p.OriginalObjectId == query.Request.OriginalObjectId);

            dbQuery = dbQuery.WhereIf(query.Request.UserId.HasValue, p => p.ObjectOwnerId == query.Request.UserId);

            dbQuery = dbQuery.WhereIf(query.Request.ParentIds != null && query.Request.ParentIds.Any(), p => query.Request.ParentIds.Contains(p.ParentId.Value));

            dbQuery = dbQuery.WhereIf(query.Request.Module.HasValue, p => p.Module == query.Request.Module);

            dbQuery = dbQuery.WhereIf(query.Request.ContentType.HasValue, p => p.ContentType == query.Request.ContentType);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.Request.PagedInfo);

            var entities = await dbQuery
                .Select(p => new BrokenLinkReportModel(p))
                .ToListAsync(cancellationToken);

            return new PagedResultDto<BrokenLinkReportModel>(totalCount, entities);
        }
    }
}
