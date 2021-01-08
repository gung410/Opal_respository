using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Domain.Constants;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;

namespace Microservice.Badge.Application.Consumers.Course
{
    [OpalConsumer("microservice.events.course.created")]
    public class CourseCreatedConsumer : OpalMessageConsumer<CourseCreatedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public CourseCreatedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(CourseCreatedMessage message)
        {
            if (message != null && message.PDActivityType == MetadataTagConstants.MicroLearningTagId)
            {
                var createdCourseRecord = await _dbContext.ActivityCollection.FirstOrDefaultAsync(p =>
                    p.SourceId == message.Id.ToString() &&
                    p.Type == ActivityType.CreatedMLU && p.UserId == message.CreatedBy);

                // if user had completed course record, we will skip it, else we insert new record.
                if (createdCourseRecord == null)
                {
                    var userActivity =
                        new UserActivity(message.CreatedBy, message.CreatedDate, message.Id, ActivityType.CreatedMLU)
                            .WithCourseInfo(new CourseInfo
                            {
                                CourseId = message.Id,
                                PdActivityType = MetadataTagConstants.MicroLearningTagId
                            });

                    await _dbContext.ActivityCollection.InsertOneAsync(userActivity);
                }
            }
        }
    }
}
