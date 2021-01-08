using System;
using System.Linq;
using Microservice.Calendar.Application.Consumers.Messages.Enums;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Domain.Extensions
{
    public static class PersonalEventExtension
    {
        public static IQueryable<PersonalEvent> HasSource(this IQueryable<PersonalEvent> query, CalendarEventSource calendarEventSource)
        {
            return query.Where(pe => pe.Source == calendarEventSource);
        }

        /// <summary>
        /// Get events of a user, with Primary Aprroval Officer (PAO) or Alternative Approval Officer (AAO).
        /// </summary>
        /// <param name="personEventsQuery"><see cref="PersonalEvent"/> query.</param>
        /// <param name="userId">User ID.</param>
        /// <param name="approvalOfficerId">PAO or AAO ID of the user.</param>
        /// <param name="userRepo"><see cref="CalendarUser"/> repository.</param>
        /// <param name="userPersonalEventRepo"><see cref="UserPersonalEvent"/> repository.</param>
        /// <returns><see cref="PersonalEvent"/> list of the user.</returns>
        public static IQueryable<PersonalEvent> OfUser(
            this IQueryable<PersonalEvent> personEventsQuery,
            Guid userId,
            Guid approvalOfficerId,
            IRepository<CalendarUser> userRepo,
            IRepository<UserPersonalEvent> userPersonalEventRepo)
        {
            var users = userRepo
                .GetAll()
                .HasId(userId)
                .BelongsToApprovalOfficer(approvalOfficerId);

            return personEventsQuery.GetEventsOfUsers(users, userPersonalEventRepo);
        }

        /// <summary>
        /// Maps events to <see cref="TeamMemberEventModel"/> list.
        /// </summary>
        /// <param name="personalEventsQuery"><see cref="PersonalEvent"/> query.</param>
        /// <param name="parentId">The learner's ID.</param>
        /// <param name="courseRepo"><see cref="Course"/> repository.</param>
        /// <returns>Events of the learner.</returns>
        public static IQueryable<TeamMemberEventModel> MapToTeamMemberEvents(
            this IQueryable<PersonalEvent> personalEventsQuery,
            Guid parentId,
            IRepository<Course> courseRepo)
        {
            return from pe in personalEventsQuery
                   join course in courseRepo.GetAll()
                       on pe.SourceParentId equals course.Id
                   orderby pe.StartAt ascending
                   select new TeamMemberEventModel
                   {
                       Id = pe.Id,
                       ParentId = parentId,
                       Title = course.CourseName,
                       SubTitle = pe.Title,
                       StartAt = pe.StartAt,
                       EndAt = pe.EndAt
                   };
        }

        /// <summary>
        /// Gets <see cref="PersonalEvent"/> query by <see cref="IQueryable{CalendarUser}"/>.
        /// </summary>
        /// <param name="personalEventsQuery"><see cref="PersonalEvent"/> query.</param>
        /// <param name="calendarUsersQuery"><see cref="CalendarUser"/> query.</param>
        /// <param name="userPersonalEventsRepo"><see cref="UserPersonalEvent"/> repository.</param>
        /// <returns><see cref="IQueryable{PersonalEvent}"/> of users.</returns>
        public static IQueryable<PersonalEvent> GetEventsOfUsers(
            this IQueryable<PersonalEvent> personalEventsQuery,
            IQueryable<CalendarUser> calendarUsersQuery,
            IRepository<UserPersonalEvent> userPersonalEventsRepo)
        {
            return from user in calendarUsersQuery
                   join upe in userPersonalEventsRepo.GetAll().GetAcceptedEvents()
                       on user.Id equals upe.UserId
                   join pe in personalEventsQuery
                       on upe.EventId equals pe.Id
                   select pe;
        }

        /// <summary>
        /// Gets user's information overviews within its events.
        /// </summary>
        /// <param name="personalEventsQuery"><see cref="PersonalEvent"/> query.</param>
        /// <param name="learnersQuery"><see cref="CalendarUser"/> query.</param>
        /// <param name="userPersonalEventsQuery"><see cref="UserPersonalEvent"/> query.</param>
        /// <returns>Events with its user's information.</returns>
        public static IQueryable<MemberEventOverviewsModel> GetMemberEventOverviews(
            this IQueryable<PersonalEvent> personalEventsQuery,
            IQueryable<CalendarUser> learnersQuery,
            IQueryable<UserPersonalEvent> userPersonalEventsQuery)
        {
            return
                from learner in learnersQuery
                join userPersonalEvent in userPersonalEventsQuery
                    on learner.Id equals userPersonalEvent.UserId
                join personalEvent in personalEventsQuery
                    on userPersonalEvent.EventId equals personalEvent.Id
                select new MemberEventOverviewsModel
                {
                    UserId = userPersonalEvent.UserId,
                    FirstName = learner.FirstName,
                    LastName = learner.LastName,
                    StartAt = personalEvent.StartAt,
                    EndAt = personalEvent.EndAt
                };
        }

        /// <summary>
        /// Get session events of the user.
        /// </summary>
        /// <param name="eventEntities">Base event as IQueryable.</param>
        /// <param name="allPersonalEvents">All personal event as IQueryable.</param>
        /// <returns>List event ids as IQueryable.</returns>
        public static IQueryable<Guid> GetSessionEvents(this IQueryable<EventEntity> eventEntities, IQueryable<UserPersonalEvent> allPersonalEvents)
        {
            // Because the sessions events does not contains participants
            // In order to get session events, so we'll get it based on SourceParentId.
            var userClassRunIds = allPersonalEvents
                .Where(x => x.Event.Source == CalendarEventSource.CourseClassRun)
                .Select(p => p.Event.SourceId);

            // Join with Event table to get session events that belong to class run.
            var personalSessionEventIds =
                from classRunId in userClassRunIds
                join eventEntity in eventEntities
                    on classRunId equals eventEntity.SourceParentId
                select eventEntity.Id;

            return personalSessionEventIds;
        }

        /// <summary>
        /// Get the events were created by the user or the auto-added events were allowed to show in the personal calendar.
        /// </summary>
        /// <param name="allPersonalEvents">All personal event.</param>
        /// <returns>List event ids as queryable.</returns>
        public static IQueryable<Guid> GetPersonalEvents(this IQueryable<UserPersonalEvent> allPersonalEvents)
        {
            return allPersonalEvents
                .Where(upe => upe.Event.Source == CalendarEventSource.StandaloneForm
                              || upe.Event.Source == CalendarEventSource.ExternalPDO
                              || upe.Event.Source == CalendarEventSource.LNA
                              || upe.Event.Source == CalendarEventSource.SelfCreated
                              || upe.Event.Source == CalendarEventSource.CourseAssignment
                              || upe.Event.Source == CalendarEventSource.CommunityWebinar
                              || upe.Event.Source == CalendarEventSource.CommunityRegular)
                .Select(upe => upe.Event.Id);
        }

        /// <summary>
        /// Get events of communities that the user belongs to.
        /// </summary>
        /// <param name="communityMemberships">Community memberships.</param>
        /// <param name="communityEvents">Community events.</param>
        /// <param name="userId">User id.</param>
        /// <returns>List event ids as queryable.</returns>
        public static IQueryable<Guid> GetCommunitiesEvents(this IQueryable<CommunityMembership> communityMemberships, IQueryable<CommunityEvent> communityEvents, Guid userId)
        {
            var userCommunities = communityMemberships.Where(cm => cm.UserId == userId);

            var communityEventIdsQuery =
                from community in userCommunities
                join communityEvent in communityEvents
                    on community.CommunityId equals communityEvent.CommunityId
                select communityEvent.Id;

            return communityEventIdsQuery;
        }
    }
}
