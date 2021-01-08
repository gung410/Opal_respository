using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("microservice.events.course.learningpath.created")]
    public class LearningPathCreatedConsumer : OpalMessageConsumer<LearningPathCreatedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public LearningPathCreatedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(LearningPathCreatedMessage message)
        {
            var existedActivity = await _dbContext
                .ActivityCollection
                .FirstOrDefaultAsync(p => p.SourceId == message.Id.ToString() && p.UserId == message.CreatedBy);

            if (existedActivity != null)
            {
                return;
            }

            var activity = new UserActivity(message.CreatedBy, message.CreatedDate, message.Id, ActivityType.CreatedLearningPath);

            await _dbContext.ActivityCollection.InsertOneAsync(activity);
        }
    }
}
