using System;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Domain.ValueObject;
using Microservice.NewsFeed.Infrastructure;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thunder.Platform.Core.Timing;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("microservice.events.course.updated")]
    public class CourseUpdatedConsumer : OpalMessageConsumer<CourseChangeMessage>
    {
        private readonly ILogger<CourseUpdatedConsumer> _logger;
        private readonly NewsFeedDbContext _dbContext;

        public CourseUpdatedConsumer(ILogger<CourseUpdatedConsumer> logger, NewsFeedDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(CourseChangeMessage message)
        {
            var existedCourse = await _dbContext.SyncedCourseCollection
                .AsQueryable()
                .Where(_ => _.CourseId == message.Id)
                .FirstOrDefaultAsync();

            if (existedCourse == null)
            {
                // TODO Support sync data from CAM module to NewsFeed. It will be removed after release 3.0
                var course = MappingCourseEntityFrom(message);
                await _dbContext.SyncedCourseCollection.InsertOneAsync(course);
                return;
            }

            // Case 1: course information has changed.
            if (existedCourse.Status == CourseStatus.Approved && message.Status == CourseStatus.Published)
            {
                await CreateNewsFeedOnCourseChangedAsync(message, UpdateCourseInfoType.CourseInfoUpdated);
            }

            // case 2: course content has changed.
            else if (existedCourse.ContentStatus == ContentStatus.Approved && message.ContentStatus == ContentStatus.Published)
            {
                await CreateNewsFeedOnCourseChangedAsync(message, UpdateCourseInfoType.CourseContentUpdated);
            }

            await UpdateNewsFeedsOnCourseInfoChanged(message, existedCourse);

            // Update course with new one
            UpdateCourseData(existedCourse, message);

            await _dbContext.SyncedCourseCollection.ReplaceOneAsync(_ => _.CourseId == message.Id, existedCourse);
        }

        private async Task CreateNewsFeedOnCourseChangedAsync(CourseChangeMessage message, UpdateCourseInfoType updateInfo)
        {
            var learnerSubscription = await _dbContext.LearnerCourseSubscriptionCollection
                .AsQueryable()
                .FirstOrDefaultAsync(_ => _.CourseId == message.Id);

            if (learnerSubscription != null)
            {
                var now = Clock.Now;
                var pdpmSuggestCourseFeeds = learnerSubscription
                    .LearnerIds
                    .Select(id => new PdpmSuggestCourseFeed
                    {
                        CreatedDate = now,
                        CourseName = message.CourseName,
                        ChangedDate = message.ChangedDate ?? now,
                        CourseId = message.Id,
                        UserId = id,
                        ThumbnailUrl = message.ThumbnailUrl,
                        UpdateInfo = updateInfo,
                        Url = $"{Course.CourseDetailUrl}/{message.Id}"
                    }).ToList();

                await _dbContext.NewsFeedCollection.InsertManyAsync(pdpmSuggestCourseFeeds);
            }
        }

        /// <summary>
        /// Update news feed on course name or thumbnail url has changed.
        /// </summary>
        /// <param name="message">Message listen from course module.</param>
        /// <param name="existedCourse">Course entity.</param>
        /// <returns>No results are returned.</returns>
        private async Task UpdateNewsFeedsOnCourseInfoChanged(
            CourseChangeMessage message,
            Course existedCourse)
        {
            if (existedCourse.IsDifferentCourseName(message.CourseName) ||
                existedCourse.IsDifferentThumbnailUrl(message.ThumbnailUrl))
            {
                var filter =
                    Builders<Domain.Entities.NewsFeed>.Filter.Eq(nameof(PdpmSuggestCourseFeed.CourseId), message.Id);

                var update = Builders<Domain.Entities.NewsFeed>.Update
                    .Set(nameof(PdpmSuggestCourseFeed.CourseName), message.CourseName)
                    .Set(nameof(PdpmSuggestCourseFeed.ThumbnailUrl), message.ThumbnailUrl);

                await _dbContext.NewsFeedCollection.UpdateManyAsync(filter, update);
            }
        }

        private void UpdateCourseData(Course course, CourseChangeMessage message)
        {
            course.DepartmentId = message.DepartmentId;
            course.CreatedBy = message.CreatedBy;
            course.CourseName = message.CourseName;
            course.PDActivityType = message.PDActivityType;
            course.LearningMode = message.LearningMode;
            course.Description = message.Description;
            course.MOEOfficerId = message.MOEOfficerId;
            course.CourseType = message.CourseType;
            course.MaxReLearningTimes = message.MaxReLearningTimes;
            course.StartDate = message.StartDate;
            course.ExpiredDate = message.ExpiredDate;
            course.FirstAdministratorId = message.FirstAdministratorId;
            course.SecondAdministratorId = message.SecondAdministratorId;
            course.PrimaryApprovingOfficerId = message.PrimaryApprovingOfficerId;
            course.AlternativeApprovingOfficerId = message.AlternativeApprovingOfficerId;
            course.Status = message.Status;
            course.CreatedDate = message.CreatedDate;
            course.Version = message.Version;
            course.ApprovalContentDate = message.ApprovalContentDate;
            course.ApprovalDate = message.ApprovalDate;
            course.ArchiveDate = message.ArchiveDate;
            course.ChangedBy = message.ChangedBy;
            course.ChangedDate = message.ChangedDate;
            course.ContentStatus = message.ContentStatus;
            course.CourseCode = message.CourseCode;
            course.PublishDate = message.PublishDate;
            course.PublishedContentDate = message.PublishedContentDate;
            course.Source = message.Source;
            course.SubmittedContentDate = message.SubmittedContentDate;
            course.SubmittedDate = message.SubmittedDate;
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
