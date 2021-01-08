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

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("microservice.events.learner.course.completed")]
    public class MicroLearningCompletedConsumer : OpalMessageConsumer<MicroLearningCompletedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public MicroLearningCompletedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(MicroLearningCompletedMessage message)
        {
            if (message != null)
            {
                var completedCourseRecord = await _dbContext.ActivityCollection.FirstOrDefaultAsync(p =>
                    p.SourceId == message.CourseId.ToString() &&
                    p.Type == ActivityType.CompleteMLU && p.UserId == message.UserId);

                // if user had completed course record, we will skip it, else we insert new record.
                if (message.Status == LearningStatus.Completed && completedCourseRecord == null)
                {
                    var userActivity =
                        new UserActivity(message.UserId, message.CompletedDate, message.CourseId, ActivityType.CompleteMLU)
                            .WithCourseInfo(new CourseInfo
                            {
                                CourseId = message.CourseId,
                                PdActivityType = MetadataTagConstants.MicroLearningTagId
                            });

                    await _dbContext.ActivityCollection.InsertOneAsync(userActivity);
                }
            }
        }
    }
}
