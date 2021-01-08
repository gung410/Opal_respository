using System;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Application.Consumers.Courses
{
    [OpalConsumer("microservice.events.course.deleted")]
    public class CourseDeletedConsumer : InboxSupportConsumer<CourseDeletedMessage>
    {
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<UserPersonalEvent> _userEventRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly ILogger<CourseDeletedConsumer> _logger;

        public CourseDeletedConsumer(
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<UserPersonalEvent> userEventRepository,
            IRepository<Course> courseRepository,
            ILogger<CourseDeletedConsumer> logger)
        {
            _personalEventRepository = personalEventRepository;
            _userEventRepository = userEventRepository;
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(CourseChangedMessage message)
        {
            var courseExisted = await _courseRepository.FirstOrDefaultAsync(x => x.Id == message.Id);
            if (courseExisted == null)
            {
                _logger.LogWarning("[CourseDeletedConsumer] Course with Id {CourseId} was not existed.", message.Id);
                return;
            }

            await _courseRepository.DeleteAsync(courseExisted);

            await DeleteEventsAndParticipantsOfCourse(message.Id);
        }

        private async Task DeleteEventsAndParticipantsOfCourse(Guid courseId)
        {
            var classRunEvents = await _personalEventRepository
                .GetAll()
                .Where(x => x.Source == CalendarEventSource.CourseClassRun && x.SourceParentId == courseId)
                .ToListAsync();
            if (!classRunEvents.Any())
            {
                return;
            }

            // Delete all class run event by Course.
            await _personalEventRepository.DeleteManyAsync(classRunEvents);

            var classRunEventIds = classRunEvents.Select(p => p.Id).ToList();
            var participants = await _userEventRepository
                .GetAll()
                .Where(x => classRunEventIds.Contains(x.EventId))
                .ToListAsync();

            // Delete all attendees of class runs event by Course.
            await _userEventRepository.DeleteManyAsync(participants);

            var classRunIds = classRunEvents.Select(p => p.SourceId).ToList();
            var sessionEvents = await _personalEventRepository
                .GetAll()
                .Where(x => x.Source == CalendarEventSource.CourseSession && x.SourceParentId.HasValue && classRunIds.Contains(x.SourceParentId.Value))
                .ToListAsync();
            if (!sessionEvents.Any())
            {
                return;
            }

            // Delete all session of class runs event by Course.
            await _personalEventRepository.DeleteManyAsync(sessionEvents);
        }
    }
}
