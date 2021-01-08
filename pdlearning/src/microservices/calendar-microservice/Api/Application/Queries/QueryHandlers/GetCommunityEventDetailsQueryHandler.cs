using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Extensions;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Queries
{
    public class GetCommunityEventDetailsQueryHandler : BaseThunderQueryHandler<GetCommunityEventDetailsByIdQuery, CommunityEventDetailsModel>
    {
        private readonly IRepository<CommunityEvent> _communityEventRepo;
        private readonly IRepository<CommunityMembership> _communityMembershipRepo;

        public GetCommunityEventDetailsQueryHandler(
            IRepository<CommunityEvent> communityEventRepo,
            IRepository<CommunityMembership> communityMembershipRepo)
        {
            _communityEventRepo = communityEventRepo;
            _communityMembershipRepo = communityMembershipRepo;
        }

        protected override async Task<CommunityEventDetailsModel> HandleAsync(GetCommunityEventDetailsByIdQuery query, CancellationToken cancellationToken)
        {
            var communityEvent = await _communityEventRepo.GetCommunityEventDetailsById(query.EventId, query.UserId, _communityMembershipRepo);
            if (communityEvent == null)
            {
                throw new EntityNotFoundException(typeof(CommunityEvent), query.EventId);
            }

            return new CommunityEventDetailsModel
            {
                CommunityId = communityEvent.CommunityId,
                CommunityTitle = communityEvent.Community.Title,
                CreatedBy = communityEvent.CreatedBy,
                Description = communityEvent.Description,
                EndAt = communityEvent.EndAt,
                Id = communityEvent.Id,
                IsAllDay = communityEvent.IsAllDay,
                Source = communityEvent.Source,
                SourceId = communityEvent.SourceId,
                StartAt = communityEvent.StartAt,
                Title = communityEvent.Title,
                Type = communityEvent.Type,
                RepeatFrequency = communityEvent.RepeatFrequency,
                RepeatUntil = communityEvent.RepeatUntil
            };
        }
    }
}
