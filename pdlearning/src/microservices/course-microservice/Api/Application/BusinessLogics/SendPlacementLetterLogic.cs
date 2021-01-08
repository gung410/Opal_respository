using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.Models;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Settings;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.BusinessLogics
{
    public class SendPlacementLetterLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<Session> _readSessionRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;

        public SendPlacementLetterLogic(
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<Session> readSessionRepository,
            IThunderCqrs thunderCqrs,
            WebAppLinkBuilder webAppLinkBuilder,
            IUserContext userContext) : base(userContext)
        {
            _readCourseRepository = readCourseRepository;
            _readClassRunRepository = readClassRunRepository;
            _readSessionRepository = readSessionRepository;
            _thunderCqrs = thunderCqrs;
            _webAppLinkBuilder = webAppLinkBuilder;
        }

        public async Task ByRegistrations(
          List<Registration> registrations,
          Guid currentUserId,
          CancellationToken cancellationToken = default)
        {
            if (!registrations.Any())
            {
                return;
            }

            var courseIds = registrations.Select(x => x.CourseId);
            var courseDict = await _readCourseRepository.GetDictionaryByIdsAsync(courseIds);

            var classRunIds = registrations.Select(x => x.ClassRunId).ToList();
            var classRunDict = await _readClassRunRepository.GetDictionaryByIdsAsync(classRunIds);

            var sessions = await _readSessionRepository.GetAllListAsync(x => classRunIds.Contains(x.ClassRunId));
            var classRunIdToSessionsDic = sessions.GroupBy(s => s.ClassRunId).ToDictionary(x => x.Key, x => x.ToList());

            var validRegistrations = registrations
                .Where(r => courseDict.ContainsKey(r.CourseId) && classRunDict.ContainsKey(r.ClassRunId)).ToList();
            await _thunderCqrs.SendEvents(
                validRegistrations
                    .Select(registration =>
                    {
                        var course = courseDict[registration.CourseId];
                        var createdBy =
                            currentUserId == course.FirstAdministratorId ||
                            currentUserId == course.SecondAdministratorId
                                ? currentUserId
                                : course.FirstAdministratorId.GetValueOrDefault();

                        return new SendPlacementLetterNotifyLearnerEvent(
                            createdBy,
                            new SendPlacementLetterNotifyLearnerPayload
                            {
                                CourseName = course.CourseName,
                                CourseCode = course.CourseCode,
                                CourseSessions = classRunIdToSessionsDic.ContainsKey(registration.ClassRunId)
                                    ? classRunIdToSessionsDic[registration.ClassRunId].OrderBy(p => p.StartDateTime)
                                        .Select(x => new SessionNotificationModel(x)).ToList()
                                    : null,
                                ActionUrl = _webAppLinkBuilder.GetCourseDetailLearnerLinkForCAMModule(registration.CourseId)
                            },
                            new List<Guid> { registration.UserId });
                    }),
                cancellationToken);
        }

        public string RenderSessionTable(List<Session> sessions)
        {
            var message = "<table><tr><th>No.</th><th>Session Date</th><th>Session Time</th><th>Venue</th></tr>";

            for (int i = 0; i < sessions.Count; i++)
            {
                var sessionModel = new SessionNotificationModel(sessions[i]);
                var item = $"<tr><td>{i + 1}</td><td>{sessionModel.SessionDate}</td><td>{sessionModel.StartTime}-{sessionModel.EndTime} </td><td> {sessionModel.Venue}</td></tr>";
                message = $"{message}{item}";
            }

            return $"{message}</table>";
        }
    }
}
