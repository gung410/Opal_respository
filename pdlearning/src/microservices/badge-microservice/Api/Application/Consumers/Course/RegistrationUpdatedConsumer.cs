using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Application.Enums;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("microservice.events.course.registration.updated")]
    public class RegistrationUpdatedConsumer : OpalMessageConsumer<RegistrationUpdatedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public RegistrationUpdatedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(RegistrationUpdatedMessage message)
        {
            if (message != null)
            {
                var completedCourseRecord = await _dbContext.ActivityCollection.FirstOrDefaultAsync(p =>
                    p.SourceId == message.Id.ToString() &&
                    p.Type == ActivityType.CompleteAnotherCourse && p.UserId == message.UserId);

                // if user had completed course record corresponding with registration id, we will skip it, else we insert new record.
                if (message.LearningStatus == LearningStatus.Completed && completedCourseRecord == null)
                {
                    var userActivity =
                        new UserActivity(message.UserId, message.ChangedDate, message.Id, ActivityType.CompleteAnotherCourse)
                            .WithCourseInfo(new CourseInfo
                            {
                                CourseId = message.CourseId,
                                ClassRunId = message.ClassRunId,
                                LearningMode = message.Course?.LearningMode,
                                PdActivityType = message.Course?.PDActivityType
                            });

                    await _dbContext.ActivityCollection.InsertOneAsync(userActivity);
                }

                // if user was marked Failed and  had completed course record corresponding with registration id, we will delete that record.
                else if (message.LearningStatus == LearningStatus.Failed && completedCourseRecord != null)
                {
                    await _dbContext.ActivityCollection.DeleteOneAsync(p => p.Id == completedCourseRecord.Id);
                }

                // if learning mode is e-learning, will save active type is CompleteElearning
                if (message.Course.LearningMode == MetadataTagConstants.ELearningTagId)
                {
                   var completedElearningeRecord = await _dbContext.ActivityCollection.FirstOrDefaultAsync(p =>
                       p.SourceId == message.Id.ToString() &&
                       p.Type == ActivityType.CompleteElearning && p.UserId == message.UserId);

                    // if user had completed course record corresponding with registration id, we will skip it, else we insert new record.
                   if (message.LearningStatus == LearningStatus.Completed && completedElearningeRecord == null)
                   {
                        var userActivity =
                            new UserActivity(message.UserId, message.ChangedDate, message.Id, ActivityType.CompleteElearning)
                                .WithCourseInfo(new CourseInfo
                                {
                                    CourseId = message.CourseId,
                                    ClassRunId = message.ClassRunId,
                                    LearningMode = message.Course?.LearningMode,
                                    PdActivityType = message.Course?.PDActivityType
                                });

                        await _dbContext.ActivityCollection.InsertOneAsync(userActivity);
                   }

                   // if user was marked Failed and  had completed course record corresponding with registration id, we will delete that record.
                   else if (message.LearningStatus == LearningStatus.Failed && completedElearningeRecord != null)
                   {
                        await _dbContext.ActivityCollection.DeleteOneAsync(p => p.Id == completedElearningeRecord.Id);
                   }
                }
            }
        }
    }
}
