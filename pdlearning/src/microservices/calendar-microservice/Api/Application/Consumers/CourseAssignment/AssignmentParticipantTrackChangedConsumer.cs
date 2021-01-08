using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers.CourseAssignment
{
    [OpalConsumer("microservice.events.course.participantassignmenttrack.created")]
    [OpalConsumer("microservice.events.course.participantassignmenttrack.updated")]
    public class AssignmentParticipantTrackChangedConsumer : InboxSupportConsumer<AssignmentParticipantTrackChangedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public AssignmentParticipantTrackChangedConsumer(IThunderCqrs thunderCqrs, IRepository<PersonalEvent> personalEventRepository)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
        }

        public async Task InternalHandleAsync(AssignmentParticipantTrackChangedMessage message)
        {
            EventStatus assignmentStatus = EventStatus.Building;
            var classRunEvent = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseClassRun && x.SourceId == message.Participant.ClassRunId);
            if (classRunEvent != null)
            {
                if (classRunEvent.Status == EventStatus.Opening)
                {
                    assignmentStatus = EventStatus.Opening;
                }
            }

            var existedEvent = await _personalEventRepository
                .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseAssignment && x.SourceId == message.Id);

            if (existedEvent == null)
            {
                await _thunderCqrs.SendCommand(BuildCreateEventCommand(message, assignmentStatus));
                return;
            }

            await _thunderCqrs.SendCommand(BuildUpdateEventCommand(message, assignmentStatus));
        }

        private CreateCourseAssignmentEventCommand BuildCreateEventCommand(AssignmentParticipantTrackChangedMessage message, EventStatus status)
        {
            var startDate = message.StartDate;
            var endDate = message.EndDate;
            return new CreateCourseAssignmentEventCommand
            {
                StartAt = startDate,
                EndAt = endDate,
                Title = message.Assignment.Title,
                Source = CalendarEventSource.CourseAssignment,
                UserId = message.UserId,
                Status = status,
                AssignmentId = message.Assignment.Id,
                AssignmentParticipantTrackId = message.Id
            };
        }

        private UpdateCourseAssignmentEventCommand BuildUpdateEventCommand(AssignmentParticipantTrackChangedMessage message, EventStatus status)
        {
            var startDate = message.StartDate;
            var endDate = message.EndDate;
            return new UpdateCourseAssignmentEventCommand
            {
                StartAt = startDate,
                EndAt = endDate,
                Title = message.Assignment.Title,
                Source = CalendarEventSource.CourseAssignment,
                UserId = message.UserId,
                Status = status,
                AssignmentId = message.Assignment.Id,
                AssignmentParticipantTrackId = message.Id
            };
        }
    }
}
