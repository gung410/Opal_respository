using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCommunityEventQueryHandler : BaseThunderQueryHandler<GetCommunityEventQuery, List<CommunityEventModel>>
    {
        private readonly IRepository<CommunityEvent> _communityEventRepository;

        public GetCommunityEventQueryHandler(
            IRepository<CommunityEvent> communityEventRepository)
        {
            _communityEventRepository = communityEventRepository;
        }

        protected override Task<List<CommunityEventModel>> HandleAsync(GetCommunityEventQuery query, CancellationToken cancellationToken)
        {
            var rangeStart = query
                .Request
                .OffsetPoint
                .AddMonths(-1 * query.Request.NumberMonthOffset);

            var rangeEnd = query.Request.OffsetPoint
                .AddMonths(query.Request.NumberMonthOffset)

                // Get day end of month.
                .AddMonths(1)
                .AddDays(-1);

            return _communityEventRepository
                .GetAll()
                .Where(e => e.CommunityId == query.CommunityId)
                .OverlapsDateTimeRange(rangeStart, rangeEnd)
                .Select(e => new CommunityEventModel
                {
                    Id = e.Id,
                    CommunityId = e.CommunityId,
                    Title = e.Title,
                    CreatedBy = e.CreatedBy,
                    CreatedAt = e.CreatedDate,
                    StartAt = e.StartAt,
                    EndAt = e.EndAt,
                    IsAllDay = e.IsAllDay,
                    Source = e.Source,
                    SourceId = e.SourceId,
                    Description = e.Description,
                    CommunityTitle = e.Community.Title,
                    RepeatFrequency = e.RepeatFrequency,
                    RepeatUntil = e.RepeatUntil
                })
                .ToListAsync(cancellationToken);
        }
    }
}
