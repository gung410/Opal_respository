using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCommunityEventsByCommunityIdQueryHandler : BaseThunderQueryHandler<GetCommunityEventsByCommunityIdQuery, PagedResultDto<CommunityEventModel>>
    {
        private readonly IRepository<CommunityEvent> _communityEventRepo;
        private readonly IRepository<CommunityMembership> _comMembershipRepo;

        public GetCommunityEventsByCommunityIdQueryHandler(
            IRepository<CommunityEvent> communityEventRepo,
            IRepository<CommunityMembership> comMembershipRepo)
        {
            _communityEventRepo = communityEventRepo;
            _comMembershipRepo = comMembershipRepo;
        }

        protected override async Task<PagedResultDto<CommunityEventModel>> HandleAsync(GetCommunityEventsByCommunityIdQuery query, CancellationToken cancellationToken)
        {
            var cmOwnerQueryIds = _comMembershipRepo
                .GetAll()
                .IsMemberOf(query.UserId, query.Request.CommunityId)
                .Select(cm => cm.CommunityId);

            var resultQuery = from cmId in cmOwnerQueryIds
                              join ce in _communityEventRepo.GetAll().Where(ce => ce.Source == query.Request.CalendarEventSource)
                              on cmId equals ce.CommunityId
                              select ce;

            int totalCount = await resultQuery.CountAsync(cancellationToken);

            var sortedResultQuery = ApplySorting(resultQuery, query.PageInfo, $"{nameof(CommunityEvent.StartAt)} DESC");
            var pagedResult = await ApplyPaging(sortedResultQuery, query.PageInfo)
                .Select(ce => new CommunityEventModel
                {
                    Id = ce.Id,
                    Title = ce.Title,
                    Type = ce.Type,
                    StartAt = ce.StartAt,
                    EndAt = ce.EndAt,
                    SourceId = ce.SourceId,
                    Source = ce.Source,
                    IsAllDay = ce.IsAllDay,
                    Description = ce.Description,
                    CreatedBy = ce.CreatedBy,
                    CommunityId = ce.CommunityId,
                    CommunityEventPrivacy = ce.CommunityEventPrivacy
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<CommunityEventModel>(totalCount, pagedResult);
        }
    }
}
