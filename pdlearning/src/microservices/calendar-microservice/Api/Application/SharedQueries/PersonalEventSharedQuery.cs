using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.SharedQueries.Abstractions;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Application.SharedQueries
{
    public class PersonalEventSharedQuery : BaseSharedQuery
    {
        private readonly IRepository<UserPersonalEvent> _userPersonalEventRepository;
        private readonly IRepository<CommunityMembership> _communityMembersRepository;
        private readonly IRepository<CommunityEvent> _communityEventRepository;
        private readonly IRepository<EventEntity> _eventRepository;

        public PersonalEventSharedQuery(
            IRepository<UserPersonalEvent> userPersonalEventRepository,
            IRepository<CommunityMembership> communityMembersRepository,
            IRepository<CommunityEvent> communityEventRepository,
            IRepository<EventEntity> eventRepository)
        {
            _userPersonalEventRepository = userPersonalEventRepository;
            _communityEventRepository = communityEventRepository;
            _communityMembersRepository = communityMembersRepository;
            _eventRepository = eventRepository;
        }

        public Task<int> CountPersonalEventByRange(Guid userId, DateTime startAt, DateTime endAt, CancellationToken cancellationToken)
        {
            return BuildGetPersonalEventQuery(userId, startAt, endAt)
               .CountAsync(cancellationToken);
        }

        public Task<List<EventModel>> GetPersonalEventByRange(Guid userId, DateTime startAt, DateTime endAt, CancellationToken cancellationToken)
        {
            return BuildGetPersonalEventQuery(userId, startAt, endAt)
                .Select(p => new EventModel
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
                    Description = p.Description,
                    Type = p.Type,
                    RepeatFrequency = p.RepeatFrequency,
                    RepeatUntil = p.RepeatUntil,
                }).ToListAsync(cancellationToken);
        }

        private IQueryable<EventEntity> BuildGetPersonalEventQuery(Guid userId, DateTime startAt, DateTime endAt)
        {
            var allEventsQuery = _userPersonalEventRepository
                .GetAllIncluding(ue => ue.Event)
                .GetOwnerOrAcceptedEvents(userId);

            var personalEventIds = allEventsQuery.GetPersonalEvents();
            var sessionEventIds = _eventRepository.GetAll().GetSessionEvents(allEventsQuery);
            var communityEventIds = _communityMembersRepository.GetAll().GetCommunitiesEvents(_communityEventRepository.GetAll(), userId);

            var allUserEventIds = personalEventIds
                .Union(sessionEventIds)
                .Union(communityEventIds);

            return _eventRepository
                .GetAll()
                .GetAvailableEvents()
                .Join(allUserEventIds, e => e.Id, eventId => eventId, (e, eventId) => e)
                .OverlapsDateTimeRange(startAt, endAt);
        }
    }
}
