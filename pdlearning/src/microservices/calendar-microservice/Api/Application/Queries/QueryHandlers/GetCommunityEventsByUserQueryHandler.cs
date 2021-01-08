using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Extensions;
using Microservice.Calendar.Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Application.Queries.QueryHandlers
{
    public class GetCommunityEventsByUserQueryHandler : BaseQueryHandler<GetCommunityEventsByUserQuery, List<CommunityEventModel>>
    {
        private readonly IRepository<CommunityMembership> _communityMembershipRepo;
        private readonly IRepository<CommunityEvent> _communityEventRepo;

        public GetCommunityEventsByUserQueryHandler(
            IRepository<CommunityMembership> communityMembershipRepo,
            IRepository<CommunityEvent> communityEventRepo)
        {
            _communityMembershipRepo = communityMembershipRepo;
            _communityEventRepo = communityEventRepo;
        }

        protected override Task<List<CommunityEventModel>> HandleAsync(GetCommunityEventsByUserQuery query, CancellationToken cancellationToken)
        {
            var rangeStart = query.Request.OffsetPoint
                .AddMonths(-query.Request.NumberMonthOffset)
                .ToFirstDayOfMonth()
                .RemoveTime();

            var rangeEnd = query.Request.OffsetPoint
                .AddMonths(query.Request.NumberMonthOffset)
                .ToLastDayOfMonth()
                .RemoveTime();

            return (from cm in _communityMembershipRepo.GetAll().Where(cm => cm.UserId == query.UserId)
                    join ce in _communityEventRepo.GetAllIncluding(p => p.Community).OverlapsDateTimeRange(rangeStart, rangeEnd)
                    on cm.CommunityId equals ce.CommunityId
                    select new CommunityEventModel
                    {
                        CommunityId = ce.CommunityId,
                        CommunityTitle = ce.Community.Title,
                        CreatedBy = ce.CreatedBy,
                        CreatedAt = ce.CreatedDate,
                        Description = ce.Description,
                        EndAt = ce.EndAt,
                        Id = ce.Id,
                        IsAllDay = ce.IsAllDay,
                        Source = ce.Source,
                        SourceId = ce.SourceId,
                        StartAt = ce.StartAt,
                        Title = ce.Title,
                        Type = ce.Type,
                        RepeatFrequency = ce.RepeatFrequency,
                        RepeatUntil = ce.RepeatUntil
                    }).Distinct().ToListAsync(cancellationToken);
        }
    }
}
