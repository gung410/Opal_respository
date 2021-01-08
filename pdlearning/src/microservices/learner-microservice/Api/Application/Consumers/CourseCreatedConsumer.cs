using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.created")]
    [OpalConsumer("microservice.events.course.cloned")]
    public class CourseCreatedConsumer : ScopedOpalMessageConsumer<CourseChangeMessage>
    {
        private readonly IRepository<Course> _courseRepository;

        public CourseCreatedConsumer(IRepository<Course> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task InternalHandleAsync(CourseChangeMessage message)
        {
            // Check if Course existed we must be bypass to avoid spam redeliver message from another system
            var anyExistingCourse = await _courseRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .AnyAsync();

            if (anyExistingCourse)
            {
                return;
            }

            // Insert a new course.
            await InsertCourseFrom(message);
        }

        private Task InsertCourseFrom(CourseChangeMessage message)
        {
            var newCourse = new Course
            {
                Id = message.Id,
                DepartmentId = message.DepartmentId,
                CreatedBy = message.CreatedBy,
                CourseName = message.CourseName,
                PDActivityType = message.PDActivityType,
                LearningMode = message.LearningMode,
                Description = message.Description,
                MOEOfficerId = message.MOEOfficerId,
                CourseType = message.CourseType,
                MaxReLearningTimes = message.MaxReLearningTimes,
                StartDate = message.StartDate,
                ExpiredDate = message.ExpiredDate,
                FirstAdministratorId = message.FirstAdministratorId,
                SecondAdministratorId = message.SecondAdministratorId,
                PrimaryApprovingOfficerId = message.PrimaryApprovingOfficerId,
                AlternativeApprovingOfficerId = message.AlternativeApprovingOfficerId,
                Status = message.Status,
                CreatedDate = message.CreatedDate,
                Version = message.Version,
                ApprovalContentDate = message.ApprovalContentDate,
                ApprovalDate = message.ApprovalDate,
                ArchiveDate = message.ArchiveDate,
                ChangedBy = message.ChangedBy,
                ChangedDate = message.ChangedDate,
                ContentStatus = message.ContentStatus,
                CourseCode = message.CourseCode,
                PublishDate = message.PublishDate,
                PublishedContentDate = message.PublishedContentDate,
                Source = message.Source,
                SubmittedContentDate = message.SubmittedContentDate,
                SubmittedDate = message.SubmittedDate,
                ThumbnailUrl = message.ThumbnailUrl
            };

            return _courseRepository.InsertAsync(newCourse);
        }
    }
}
