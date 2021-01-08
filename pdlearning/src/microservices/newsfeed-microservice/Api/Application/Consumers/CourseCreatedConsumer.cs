using System;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Infrastructure;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("microservice.events.course.cloned")]
    [OpalConsumer("microservice.events.course.created")]
    public class CourseCreatedConsumer : OpalMessageConsumer<CourseChangeMessage>
    {
        private readonly NewsFeedDbContext _dbContext;

        public CourseCreatedConsumer(NewsFeedDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(CourseChangeMessage message)
        {
            var courseExisted = await _dbContext
                .SyncedCourseCollection
                .AsQueryable()
                .AnyAsync(_ => _.CourseId == message.Id);

            if (courseExisted)
            {
                return;
            }

            var course = MappingCourseEntityFrom(message);

            await _dbContext.SyncedCourseCollection.InsertOneAsync(course);
        }

        private Course MappingCourseEntityFrom(CourseChangeMessage message)
        {
            return new Course
            {
                Id = Guid.NewGuid(),
                CourseId = message.Id,
                ThumbnailUrl = message.ThumbnailUrl,
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
                SubmittedDate = message.SubmittedDate
            };
        }
    }
}
