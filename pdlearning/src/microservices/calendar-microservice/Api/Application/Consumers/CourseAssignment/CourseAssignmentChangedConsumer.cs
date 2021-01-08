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
    [OpalConsumer("microservice.events.course.assignment.created")]
    [OpalConsumer("microservice.events.course.assignment.updated")]
    public class CourseAssignmentChangedConsumer : InboxSupportConsumer<CourseAssignmentChangedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<PersonalEvent> _personalEventRepository;

        public CourseAssignmentChangedConsumer(IThunderCqrs thunderCqrs, IRepository<PersonalEvent> personalEventRepository)
        {
            _thunderCqrs = thunderCqrs;
            _personalEventRepository = personalEventRepository;
        }

        public async Task InternalHandleAsync(CourseAssignmentChangedMessage message)
        {
            if (message.ClassRunId.HasValue && message.ClassRunId.Value != Guid.Empty)
            {
                var existedEvent = await _personalEventRepository
                    .FirstOrDefaultAsync(x => x.Source == CalendarEventSource.CourseAssignment && x.SourceId == message.Id);
                if (existedEvent == null)
                {
                    await _thunderCqrs.SendCommand(BuildCreateEventCommand(message));
                    return;
                }

                await _thunderCqrs.SendCommand(BuildUpdateEventCommand(message));
            }
        }

        private CreateCourseAssignmentBaseEventCommand BuildCreateEventCommand(CourseAssignmentChangedMessage message)
        {
            return new CreateCourseAssignmentBaseEventCommand
            {
                StartAt = new DateTime(year: message.StartDate.Year, month: message.StartDate.Month, day: message.StartDate.Day, 0, 0, 0),
                EndAt = new DateTime(year: message.EndDate.Year, month: message.EndDate.Month, day: message.EndDate.Day, 23, 59, 59),
                Title = message.Title,
                Source = CalendarEventSource.CourseAssignment,
                Status = EventStatus.Opening,
                AssignmentId = message.Id,
                ClassRunId = message.ClassRunId.Value
            };
        }

        private UpdateCourseAssignmentBaseEventCommand BuildUpdateEventCommand(CourseAssignmentChangedMessage message)
        {
            return new UpdateCourseAssignmentBaseEventCommand
            {
                StartAt = new DateTime(year: message.StartDate.Year, month: message.StartDate.Month, day: message.StartDate.Day, 0, 0, 0),
                EndAt = new DateTime(year: message.EndDate.Year, month: message.EndDate.Month, day: message.EndDate.Day, 23, 59, 59),
                Title = message.Title,
                Source = CalendarEventSource.CourseAssignment,
                Status = EventStatus.Opening,
                AssignmentId = message.Id,
                ClassRunId = message.ClassRunId.Value
            };
        }
    }
}
