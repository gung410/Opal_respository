using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Events.WebinarEvents.WebinarMeetingEvent;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Course.Application.BusinessLogics
{
    public class BookWebinarMeetingLogic : BaseBusinessLogic
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassrunRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly GetAggregatedSessionSharedQuery _getAggregatedSessionSharedQuery;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;

        public BookWebinarMeetingLogic(
            IThunderCqrs thunderCqrs,
            IReadOnlyRepository<CourseUser> readUserRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            IReadOnlyRepository<ClassRun> readClassrunRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            GetAggregatedSessionSharedQuery getAggregatedSessionSharedQuery,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            IUserContext userContext) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
            _readUserRepository = readUserRepository;
            _readSessionRepository = readSessionRepository;
            _readClassrunRepository = readClassrunRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _getAggregatedSessionSharedQuery = getAggregatedSessionSharedQuery;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
        }

        public async Task BookMeeting(IQueryable<Session> sessionsQuery, WebinarMeetingAction action, CancellationToken cancellationToken = default)
        {
            var aggregatedSessions = await _getAggregatedSessionSharedQuery.ByQuery(sessionsQuery, cancellationToken);
            await BookMeeting(aggregatedSessions, action, cancellationToken);
        }

        public Task BookMeeting(List<Session> sessions, WebinarMeetingAction action, CancellationToken cancellationToken = default)
        {
            var sessionIds = sessions.Select(x => x.Id).ToList();
            return BookMeeting(_readSessionRepository.GetAll().Where(p => sessionIds.Contains(p.Id)), action, cancellationToken);
        }

        public async Task UpdateMeeting(List<Registration> newParticipantRegistrations, CancellationToken cancellationToken = default)
        {
            var classrunIds = newParticipantRegistrations
                .Select(p => p.ClassRunId)
                .Distinct()
                .ToList();

            var sessions = _readSessionRepository
                .GetAll()
                .Where(Session.CanUpdateMeetingExpr())
                .Where(p => classrunIds.Contains(p.ClassRunId));

            var aggregatedSessions = await _getAggregatedSessionSharedQuery.ByQuery(sessions, cancellationToken);
            var sessionClassRunIdToAttendeeInfos = await GetSessionClassRunIdToAttendeeInfo(aggregatedSessions, newParticipantRegistrations, cancellationToken);

            var events = aggregatedSessions
                .Select(p => WebinarMeetingRequest.Create(p.Session, sessionClassRunIdToAttendeeInfos[p.ClassRun.Id]))
                .Select(x => new WebinarMeetingEvent(x, WebinarMeetingAction.Update))
                .ToList();

            await _thunderCqrs.SendEvents(events);
        }

        private async Task BookMeeting(List<SessionAggregatedEntityModel> aggregatedSessions, WebinarMeetingAction action, CancellationToken cancellationToken = default)
        {
            var sessionClassRunIdToAttendeeInfo = await GetSessionClassRunIdToAttendeeInfo(aggregatedSessions, new List<Registration>(), cancellationToken);

            var events = aggregatedSessions
                .Select(p => WebinarMeetingRequest.Create(p.Session, sessionClassRunIdToAttendeeInfo[p.ClassRun.Id]))
                .Select(x => new WebinarMeetingEvent(x, action));

            await _thunderCqrs.SendEvents(events, cancellationToken);
        }

        private async Task<Dictionary<Guid, List<WebinarMeetingAttendeeInfoRequest>>> GetSessionClassRunIdToAttendeeInfo(
            List<SessionAggregatedEntityModel> aggregatedSessions,
            List<Registration> newWillBeAddedParticipantRegistrations,
            CancellationToken cancellationToken = default)
        {
            var sessionClassRunIdToAttendeeUsers = await GetSessionClassRunIdToAttendeeUsers(aggregatedSessions, newWillBeAddedParticipantRegistrations, cancellationToken);

            return sessionClassRunIdToAttendeeUsers.ToDictionary(
                p => p.Key,
                p => p.Value
                    .Select(user => new WebinarMeetingAttendeeInfoRequest
                    {
                        Id = user.Id,
                        IsModerator = user.SystemRoles.Contains(UserRoles.CourseFacilitator)
                    })
                    .ToList());
        }

        private async Task<Dictionary<Guid, List<CourseUser>>> GetSessionClassRunIdToAttendeeUsers(
            List<SessionAggregatedEntityModel> aggregatedSessions,
            List<Registration> newWillBeAddedParticipantRegistrations,
            CancellationToken cancellationToken = default)
        {
            // Get management attendee users grouped by courses of all sessions
            var managementUserIds = aggregatedSessions.SelectMany(p => p.Course.GetManagementInvoledUserIds()).Distinct();
            var managementAttendeeUsers = (await _readUserRepository.GetAll().Where(p => managementUserIds.Contains(p.Id)).ToListAsync(cancellationToken))
                .ToDictionary(p => p.Id);
            var managementAttendeeUsersGroupByCourse = aggregatedSessions
                .Select(p => p.Course)
                .DistinctBy(p => p.Id)
                .ToDictionary(
                    p => p.Id,
                    p => p.GetManagementInvoledUserIds()
                        .Select(userId => managementAttendeeUsers.GetValueOrDefault(userId))
                        .Where(user => user != null));

            // Get participant users grouped by classrun of all sessions
            var classRunIds = aggregatedSessions.Select(p => p.ClassRun.Id).Distinct();
            var classRunIdToParticipantUsers =
                (await _readClassrunRepository.GetAll().Where(p => classRunIds.Contains(p.Id))
                    .GroupJoin(
                        _readRegistrationRepository.GetAll().Where(Registration.IsParticipantExpr()),
                        p => p.Id,
                        p => p.ClassRunId,
                        (classRun, registrations) => new { classRun, registrations })
                    .SelectMany(
                        p => p.registrations.DefaultIfEmpty(),
                        (gj, registration) => new { gj.classRun, registration })
                    .GroupJoin(
                        _readUserRepository.GetAll(),
                        p => p.registration.UserId,
                        p => p.Id,
                        (gj, user) => new { gj.classRun, user })
                    .SelectMany(
                        p => p.user.DefaultIfEmpty(),
                        (gj, user) => new { gj.classRun, user })
                    .ToListAsync(cancellationToken))
                .GroupBy(p => p.classRun, p => p.user);

            // Get class run to new will be added participant users dictionary
            var classRunIdToNewWillBeAddedParticipantUsers =
                (await _getAggregatedRegistrationSharedQuery
                    .FullByRegistrations(
                        newWillBeAddedParticipantRegistrations,
                        cancellationToken))
                .GroupBy(p => p.Registration.ClassRunId)
                .ToDictionary(p => p.Key, p => p.SelectList(aggregatedSession => aggregatedSession.User));

            return classRunIdToParticipantUsers.ToDictionary(
                p => p.Key.Id,
                p =>
                {
                    var classrun = p.Key;

                    // Get management users of course of current classrun
                    var classRunCourseManagementAttendeeUsers = managementAttendeeUsersGroupByCourse[classrun.CourseId];

                    // Combine existed users with management users and new will be added participant users
                    return p.Where(user => user != null)
                        .Concat(classRunCourseManagementAttendeeUsers)
                        .Concat(classRunIdToNewWillBeAddedParticipantUsers.GetValueOrDefault(classrun.Id, new List<CourseUser>()))
                        .DistinctBy(user => user.Id)
                        .ToList();
                });
        }
    }
}
