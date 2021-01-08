using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCommunityEventByIdQueryHandler : BaseThunderQueryHandler<GetCommunityEventByIdQuery, CommunityEventModel>
    {
        private readonly IRepository<CommunityEvent> _communityEventRepository;

        public GetCommunityEventByIdQueryHandler(
            IRepository<CommunityEvent> communityEventRepository)
        {
            _communityEventRepository = communityEventRepository;
        }

        protected override Task<CommunityEventModel> HandleAsync(GetCommunityEventByIdQuery query, CancellationToken cancellationToken)
        {
            return _communityEventRepository
                .GetAll()
                .Where(p => p.Id == query.EventId)
                .Select(p => new CommunityEventModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    CreatedBy = p.CreatedBy,
                    CreatedAt = p.CreatedDate,
                    StartAt = p.StartAt,
                    EndAt = p.EndAt,
                    IsAllDay = p.IsAllDay,
                    Source = p.Source,
                    SourceId = p.SourceId,
                    CommunityId = p.CommunityId,
                    CommunityTitle = p.Community.Title,
                    Description = p.Description,
                    CommunityEventPrivacy = p.CommunityEventPrivacy,
                    RepeatFrequency = p.RepeatFrequency,
                    RepeatUntil = p.RepeatUntil
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
