using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class SearchAnnouncementTemplatesQueryHandler : BaseQueryHandler<SearchAnnouncementTemplatesQuery, PagedResultDto<AnnouncementTemplateModel>>
    {
        private readonly IReadOnlyRepository<AnnouncementTemplate> _readAnnouncementTemplateRepository;
        private readonly GetFullTextFilteredEntitiesSharedQuery _getFullTextFilteredEntitiesSharedQuery;

        public SearchAnnouncementTemplatesQueryHandler(
            IReadOnlyRepository<AnnouncementTemplate> readAnnouncementTemplateRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            GetFullTextFilteredEntitiesSharedQuery getFullTextFilteredEntitiesSharedQuery,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAnnouncementTemplateRepository = readAnnouncementTemplateRepository;
            _getFullTextFilteredEntitiesSharedQuery = getFullTextFilteredEntitiesSharedQuery;
        }

        protected override async Task<PagedResultDto<AnnouncementTemplateModel>> HandleAsync(SearchAnnouncementTemplatesQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readAnnouncementTemplateRepository.GetAll();

            // Full search text need to be put at the first to prevent CONTAINS SQL need to be applied directly into AnnouncementTemplate table, not Join query
            dbQuery = _getFullTextFilteredEntitiesSharedQuery.BySearchText(dbQuery, query.SearchText, announcement => announcement.FullTextSearch);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            if (query.PageInfo != null && query.PageInfo.MaxResultCount == 0)
            {
                return new PagedResultDto<AnnouncementTemplateModel>(totalCount);
            }

            dbQuery = dbQuery.OrderByDescending(p => p.FullTextSearchKey);

            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            return new PagedResultDto<AnnouncementTemplateModel>(totalCount, dbQuery.Select(p => new AnnouncementTemplateModel(p)).ToList());
        }
    }
}
