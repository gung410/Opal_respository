using System;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Application.Consumers.Messages.Helpers;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Calendar.Application.Consumers.Courses
{
    [OpalConsumer("microservice.events.course.created")]
    [OpalConsumer("microservice.events.course.updated")]
    public class CourseChangedConsumer : InboxSupportConsumer<CourseChangedMessage>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<PersonalEvent> _personalEventRepository;
        private readonly IRepository<ClassRun> _classRunRepository;

        public CourseChangedConsumer(
            IRepository<Course> courseRepository,
            IRepository<PersonalEvent> personalEventRepository,
            IRepository<ClassRun> classRunRepository)
        {
            _courseRepository = courseRepository;
            _personalEventRepository = personalEventRepository;
            _classRunRepository = classRunRepository;
        }

        public async Task InternalHandleAsync(CourseChangedMessage message)
        {
            var courseExisted = await _courseRepository.FirstOrDefaultAsync(x => x.Id == message.Id);
            if (courseExisted == null)
            {
                var newCourse = new Course
                {
                    Id = message.Id,
                    CourseName = message.CourseName,
                    Status = message.Status
                };

                await _courseRepository.InsertAsync(newCourse);
            }
            else
            {
                courseExisted.CourseName = message.CourseName;
                courseExisted.Status = message.Status;
                await _courseRepository.UpdateAsync(courseExisted);
            }

            await UpdateStatusClassrunAndSessionEventsByCourse(message.Id, message.Status);
        }

        /// <summary>
        /// Update status classrun and session events base on Course and ClassRun status.
        /// Events has Opening status when Course and ClassRun is Publish.
        /// </summary>
        /// <param name="courseId">CourseId param.</param>
        /// <param name="courseStatus">Course status.</param>
        /// <returns>.</returns>
        private async Task UpdateStatusClassrunAndSessionEventsByCourse(Guid courseId, CourseStatus courseStatus)
        {
            var classRuns = await _classRunRepository
                .GetAll()
                .Where(x => x.CourseId == courseId && x.Status == ClassRunStatus.Published)
                .ToListAsync();

            foreach (var classRun in classRuns)
            {
                var classRunEventStatus = ClassRunStatusMapper.GetClassRunEventStatus(classRun.Status, courseStatus);
                var classRunEvent = await _personalEventRepository
                    .FirstOrDefaultAsync(x =>
                        x.Source == CalendarEventSource.CourseClassRun &&
                        x.SourceId == classRun.Id &&
                        x.SourceParentId.HasValue &&
                        x.SourceParentId == courseId);

                if (classRunEvent == null)
                {
                    continue;
                }

                classRunEvent.Status = classRunEventStatus;
                await _personalEventRepository.UpdateAsync(classRunEvent);

                var sessionEvents = await _personalEventRepository
                    .GetAll()
                    .Where(x => x.Source == CalendarEventSource.CourseSession)
                    .Where(x => x.SourceParentId.HasValue && x.SourceParentId.Value == classRun.Id)
                    .ToListAsync();

                if (!sessionEvents.Any())
                {
                    continue;
                }

                sessionEvents.ForEach(p => p.Status = classRunEventStatus);
                await _personalEventRepository.UpdateManyAsync(sessionEvents);
            }
        }
    }
}
