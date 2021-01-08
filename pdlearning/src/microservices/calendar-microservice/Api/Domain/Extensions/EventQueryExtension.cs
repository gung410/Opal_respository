using System;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Domain.Extensions
{
    public static class EventQueryExtension
    {
        public static IQueryable<EventEntity> GetAllByUser(
            this IRepository<EventEntity> eventRepository,
            IRepository<UserPersonalEvent> userEventRepository,
            Guid userId)
        {
            return eventRepository
                .GetAll()
                .GetAllByUser(userEventRepository, userId);
        }

        public static IQueryable<EventEntity> GetAllByUser(
            this IQueryable<EventEntity> eventQueryable,
            IRepository<UserPersonalEvent> userEventRepository,
            Guid userId)
        {
            var userEventsQuery = userEventRepository
                .GetAll()
                .Where(p => p.UserId == userId);

            return userEventsQuery
                .Join(
                    eventQueryable,
                    ue => ue.EventId,
                    e => e.Id,
                    (ue, e) => e);
        }

        public static Task<CommunityEvent> GetCommunityEventDetailsById(
            this IRepository<CommunityEvent> communityEventRepo,
            Guid eventId,
            Guid userId,
            IRepository<CommunityMembership> communityMembershipRepo)
        {
            return communityEventRepo.GetAll().Where(ce => ce.Id == eventId)
                .Join(
                    communityMembershipRepo.GetAll().Where(cm => cm.UserId == userId),
                    ce => ce.CommunityId,
                    cm => cm.CommunityId,
                    (ce, cm) => ce)
                .FirstOrDefaultAsync();
        }
    }
}
