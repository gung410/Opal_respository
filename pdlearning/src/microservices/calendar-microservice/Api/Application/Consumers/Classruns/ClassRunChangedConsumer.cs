using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Application.Consumers.Messages.Helpers;
using Microservice.Calendar.Application.Consumers.Messages.Models;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microservice.Calendar.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers.Classruns
{
    [OpalConsumer("microservice.events.course.classrun.created")]
    [OpalConsumer("microservice.events.course.classrun.updated")]
    public class ClassRunChangedConsumer : InboxSupportConsumer<ClassRunChangedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<ClassRun> _classRunRepository;

        public ClassRunChangedConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<ClassRun> classRunRepository)
        {
            _personalEventRepository = personalEventRepository;
            _thunderCqrs = thunderCqrs;
            _classRunRepository = classRunRepository;
        }

        public async Task InternalHandleAsync(ClassRunChangedMessage message)
        {
            await UpsertClassRun(message);

            var classRunEventStatus = ClassRunStatusMapper.GetClassRunEventStatus(message.Status, message.Course.Status);
            var classRunEventExisted = await _personalEventRepository
                    .FirstOrDefaultAsync(x => x.SourceId == message.Id && x.Source == CalendarEventSource.CourseClassRun);

            if (classRunEventExisted == null)
            {
                await _thunderCqrs.SendCommand(BuildCreateClassRunCommand(message, classRunEventStatus));
            }
            else
            {
                await _thunderCqrs.SendCommand(BuildUpdateClassRunCommand(message, classRunEventStatus));

                // When a class run is updated, all the assignment events which referenced to it should be updated too.
                // Event linked with structures:
                // - SourceId: ParticipantTrackId
                // - SourceParentId: AssignmentId
                // Assignment Base Event:
                // - SourceId: Assignment Id
                // - SourceParentId: ClassRun Id
                var relatedBaseEventIds = await _personalEventRepository
                    .GetAll()
                    .Where(x => x.Source == CalendarEventSource.CourseAssignment && x.SourceParentId == message.Id)
                    .Where(x => x.SourceId.HasValue)
                    .Select(x => x.SourceId)
                    .Distinct()
                    .ToListAsync();
                if (relatedBaseEventIds.Count > 0)
                {
                    var relatedAssignmentEvents = await _personalEventRepository
                        .GetAll()
                        .Where(x => x.Source == CalendarEventSource.CourseAssignment && relatedBaseEventIds.Contains(x.SourceParentId))
                        .Select(x => x.Id)
                        .ToListAsync();
                    if (relatedAssignmentEvents.Count > 0)
                    {
                        await _thunderCqrs.SendCommand(BuildUpdateCourseAssignmentEventByIdCommand(relatedAssignmentEvents, message));
                    }
                }
            }

            await UpdateSessionEventsByClassRunId(message.Sessions, message.Id, classRunEventStatus);
        }

        private CreateClassRunEventCommand BuildCreateClassRunCommand(ClassRunChangedMessage message, EventStatus status)
        {
            return new CreateClassRunEventCommand
            {
                ClassRunId = message.Id,
                ClassTitle = message.ClassTitle,
                StartDateTime = message.StartDateTime,
                EndDateTime = message.EndDateTime,
                Status = status,
                CourseId = message.Course.Id
            };
        }

        private UpdateClassRunEventCommand BuildUpdateClassRunCommand(ClassRunChangedMessage message, EventStatus status)
        {
            return new UpdateClassRunEventCommand
            {
                ClassRunId = message.Id,
                ClassTitle = message.ClassTitle,
                StartDateTime = message.StartDateTime,
                EndDateTime = message.EndDateTime,
                Status = status,
                CourseId = message.Course.Id
            };
        }

        private async Task UpdateSessionEventsByClassRunId(List<SessionModel> sessions, Guid classRunId, EventStatus status)
        {
            var sessionsExisted = await _personalEventRepository
                .GetAll()
                .Where(x => x.SourceParentId == classRunId && x.Source == CalendarEventSource.CourseSession)
                .ToListAsync();

            if (!sessionsExisted.Any() && !sessions.Any())
            {
                return;
            }

            if (!sessions.Any() && sessionsExisted.Any())
            {
                await _personalEventRepository.DeleteManyAsync(sessionsExisted);
                return;
            }

            if (sessions.Any() && !sessionsExisted.Any())
            {
                var newSessionsEvent = sessions.Select(p => new PersonalEvent()
                    .WithTitle(p.SessionTitle)
                    .WithTime(p.StartDateTime, p.EndDateTime)
                    .FromSource(CalendarEventSource.CourseSession, p.Id)
                    .FromSourceParent(classRunId)
                    .WithStatus(status));
                await _personalEventRepository.InsertManyAsync(newSessionsEvent);
                return;
            }

            var existedSessionIds = sessionsExisted.Select(p => p.Id).ToList();
            var newSessionIds = sessions.Select(p => p.Id).ToList();

            var toBeDeleteSessions = sessionsExisted.Where(x => !newSessionIds.Contains(x.Id)).ToList();
            if (toBeDeleteSessions.Any())
            {
                await _personalEventRepository.DeleteManyAsync(toBeDeleteSessions);
            }

            var toBeCreateSessions = sessions.Where(x => !existedSessionIds.Contains(x.Id)).ToList();
            if (toBeCreateSessions.Any())
            {
                var createSessionsEvent = toBeCreateSessions.Select(p => new PersonalEvent()
                    .WithTitle(p.SessionTitle)
                    .WithTime(p.StartDateTime, p.EndDateTime)
                    .FromSource(CalendarEventSource.CourseSession, p.Id)
                    .FromSourceParent(classRunId)
                    .WithStatus(status));
                await _personalEventRepository.InsertManyAsync(createSessionsEvent);
            }

            var toBeUpdateSessions = sessionsExisted.Where(x => newSessionIds.Contains(x.Id)).ToList();
            if (toBeUpdateSessions.Any())
            {
                var updateSessionsEvent = new List<PersonalEvent>();
                foreach (var item in toBeUpdateSessions)
                {
                    var infoUpdate = sessions.First(x => x.Id == item.SourceId);
                    item.Title = infoUpdate.SessionTitle;
                    item.StartAt = infoUpdate.StartDateTime;
                    item.EndAt = infoUpdate.EndDateTime;
                    item.Status = status;
                    updateSessionsEvent.Add(item);
                }

                await _personalEventRepository.UpdateManyAsync(updateSessionsEvent);
            }
        }

        private UpdateCourseAssignmentEventByIdsCommand BuildUpdateCourseAssignmentEventByIdCommand(List<Guid> eventIds, ClassRunChangedMessage message)
        {
            return new UpdateCourseAssignmentEventByIdsCommand
            {
                Status = message.Status == ClassRunStatus.Published ? EventStatus.Opening : EventStatus.Building,
                AssignmentEventIds = eventIds
            };
        }

        private async Task UpsertClassRun(ClassRunChangedMessage message)
        {
            var classRunExisted = await _classRunRepository.FirstOrDefaultAsync(message.Id);

            if (classRunExisted == null)
            {
                await _classRunRepository.InsertAsync(new ClassRun
                {
                    Id = message.Id,
                    CourseId = message.Course.Id,
                    Status = message.Status
                });
            }
            else
            {
                classRunExisted.Status = message.Status;
                await _classRunRepository.UpdateAsync(classRunExisted);
            }
        }
    }
}
