using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Application.Events.Todo;
using Microservice.Learner.Application.SharedQueries.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.updated")]
    public class CourseUpdatedConsumer : ScopedOpalMessageConsumer<CourseChangeMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<Course> _courseRepository;
        private readonly ILogger<CourseUpdatedConsumer> _logger;
        private readonly IReadMyCourseShared _readMyCourseShared;
        private readonly IRepository<MyCourse> _myCourseRepository;
        private readonly IRepository<LearnerLearningPath> _learnerLearningPathRepository;
        private readonly IRepository<LearnerLearningPathCourse> _learnerLearningPathCourseRepository;

        public CourseUpdatedConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<Course> courseRepository,
            ILogger<CourseUpdatedConsumer> logger,
            IReadMyCourseShared readMyCourseShared,
            IRepository<MyCourse> myCourseRepository,
            IRepository<LearnerLearningPath> learnerLearningPathRepository,
            IRepository<LearnerLearningPathCourse> learnerLearningPathCourseRepository)
        {
            _logger = logger;
            _thunderCqrs = thunderCqrs;
            _courseRepository = courseRepository;
            _myCourseRepository = myCourseRepository;
            _readMyCourseShared = readMyCourseShared;
            _learnerLearningPathRepository = learnerLearningPathRepository;
            _learnerLearningPathCourseRepository = learnerLearningPathCourseRepository;
        }

        public async Task InternalHandleAsync(CourseChangeMessage message)
        {
            var existingCourse = await _courseRepository.FirstOrDefaultAsync(p => p.Id == message.Id);
            if (existingCourse == null)
            {
                // Implement Idempotent to avoid duplicate data come when the message can redeliver in RabbitMQ
                _logger.LogError(message: "Course not found with course id: {MessageId}", message.Id);
                return;
            }

            // TODO: ChangedDate,MessageCreatedDate is nullable type, should we have logic to check and return?
            var courseChangedDate = existingCourse.ChangedDate.Value.ToUniversalTime();
            var messageCreatedDate = message.MessageCreatedDate.Value.ToUniversalTime();
            var messageChangeDate = message.ChangedDate.Value.ToUniversalTime();

            if (courseChangedDate > messageCreatedDate || messageChangeDate == courseChangedDate)
            {
                // Implement Idempotent to avoid duplicate data come when the message can redeliver in RabbitMQ
                _logger.LogError("The message comes from another system is out of date: {ExistingCourseChangedDate} {MessageCreatedDate}", existingCourse.ChangedDate.Value.ToUniversalTime(), message.MessageCreatedDate.Value.ToUniversalTime());
                return;
            }

            if (existingCourse.IsMicroLearning())
            {
                // MyCourse update is required prior to course update to avoid changing existing course data
                await UpdateMyCourseOnContentChanged(existingCourse, message);
            }

            // Update existing course.
            await UpdateExistingCourseFrom(existingCourse, message);

            // Send notification.
            await SendNotificationOnCourseArchived(existingCourse);
        }

        private async Task UpdateMyCourseOnContentChanged(Course course, CourseChangeMessage message)
        {
            if (!(course.ApprovalContentDate.HasValue
                  && course.ContentStatus == ContentStatus.Approved
                  && message.ContentStatus == ContentStatus.Published))
            {
                return;
            }

            // After the course has not been published, only courses of type MicroLearning can change the content
            // We need to inform learners that the content of the course has been changed by flags
            var myCourses = await _readMyCourseShared
                .FilterByCourseIdAndCourseTypeQuery(message.Id, LearningCourseType.Microlearning)
                .ToListAsync();

            if (!myCourses.Any())
            {
                return;
            }

            myCourses.ForEach(myCourse =>
            {
                myCourse.HasContentChanged = true;
            });

            await _myCourseRepository.UpdateManyAsync(myCourses);
        }

        private async Task SendNotificationOnCourseArchived(Course existingCourse)
        {
            if (existingCourse.Status != CourseStatus.Archived)
            {
                return;
            }

            var learningPathOwnerIds = await GetLearningPathOwnerIds(existingCourse);

            if (!learningPathOwnerIds.Any())
            {
                return;
            }

            var archivedCourseNotificationEvent = new ArchivedCourseNotificationEvent(
                existingCourse.CreatedBy,
                new ArchivedCourseNotificationPayload
                {
                    CourseTitle = existingCourse.CourseName,
                    CourseType = existingCourse.CourseType.ToString()
                },
                learningPathOwnerIds);

            await _thunderCqrs.SendEvent(archivedCourseNotificationEvent);
        }

        private Task<List<Guid>> GetLearningPathOwnerIds(Course existingCourse)
        {
            var learnerLearningPathCourseQuery = _learnerLearningPathCourseRepository
                .GetAll()
                .Where(p => p.CourseId == existingCourse.Id);

            return _learnerLearningPathRepository
                .GetAll()
                .Join(
                    learnerLearningPathCourseQuery,
                    lp => lp.Id,
                    lpc => lpc.LearningPathId,
                    (learningPath, learnerLearningPathCourse) => learningPath)
                .Select(p => p.CreatedBy)
                .Distinct()
                .ToListAsync();
        }

        private Task UpdateExistingCourseFrom(Course existingCourse, CourseChangeMessage message)
        {
            existingCourse.DepartmentId = message.DepartmentId;
            existingCourse.CreatedBy = message.CreatedBy;
            existingCourse.CourseName = message.CourseName;
            existingCourse.PDActivityType = message.PDActivityType;
            existingCourse.LearningMode = message.LearningMode;
            existingCourse.Description = message.Description;
            existingCourse.MOEOfficerId = message.MOEOfficerId;
            existingCourse.CourseType = message.CourseType;
            existingCourse.MaxReLearningTimes = message.MaxReLearningTimes;
            existingCourse.StartDate = message.StartDate;
            existingCourse.ExpiredDate = message.ExpiredDate;
            existingCourse.FirstAdministratorId = message.FirstAdministratorId;
            existingCourse.SecondAdministratorId = message.SecondAdministratorId;
            existingCourse.PrimaryApprovingOfficerId = message.PrimaryApprovingOfficerId;
            existingCourse.AlternativeApprovingOfficerId = message.AlternativeApprovingOfficerId;
            existingCourse.Status = message.Status;
            existingCourse.CreatedDate = message.CreatedDate;
            existingCourse.Version = message.Version;
            existingCourse.ApprovalContentDate = message.ApprovalContentDate;
            existingCourse.ApprovalDate = message.ApprovalDate;
            existingCourse.ArchiveDate = message.ArchiveDate;
            existingCourse.ChangedBy = message.ChangedBy;
            existingCourse.ChangedDate = message.ChangedDate;
            existingCourse.ContentStatus = message.ContentStatus;
            existingCourse.CourseCode = message.CourseCode;
            existingCourse.PublishDate = message.PublishDate;
            existingCourse.PublishedContentDate = message.PublishedContentDate;
            existingCourse.Source = message.Source;
            existingCourse.SubmittedContentDate = message.SubmittedContentDate;
            existingCourse.SubmittedDate = message.SubmittedDate;
            existingCourse.ThumbnailUrl = message.ThumbnailUrl;

            return _courseRepository.UpdateAsync(existingCourse);
        }
    }
}
