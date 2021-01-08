using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class SearchAnnouncementQueryHandler : BaseQueryHandler<SearchAnnouncementQuery, PagedResultDto<AnnouncementModel>>
    {
        private readonly IReadOnlyRepository<Announcement> _readAnnouncementRepository;

        public SearchAnnouncementQueryHandler(
            IReadOnlyRepository<Announcement> readAnnouncementRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readAnnouncementRepository = readAnnouncementRepository;
        }

        protected override async Task<PagedResultDto<AnnouncementModel>> HandleAsync(SearchAnnouncementQuery query, CancellationToken cancellationToken)
        {
            var dbQuery = _readAnnouncementRepository.GetAll()
                .Where(x => x.CourseId == query.CourseId && x.ClassrunId == query.ClassRunId);

            dbQuery = query.Filter?.ContainFilters?.Aggregate(
                dbQuery,
                (current, containFilter) =>
                     current.BuildContainFilter(
                        containFilter.Field,
                        containFilter.Values,
                        null,
                        containFilter.NotContain)) ?? dbQuery;

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

            dbQuery = dbQuery.OrderByDescending(p => p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.PagedInfo);

            var entities = await dbQuery.Select(p => new AnnouncementModel(p)).ToListAsync(cancellationToken);

            return new PagedResultDto<AnnouncementModel>(totalCount, entities);
        }
    }
}
