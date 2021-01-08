using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Application.Enums;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("microservice.events.learner.bookmark.created")]
    public class LearningPathBookMarkedConsumer : OpalMessageConsumer<LearningPathBookMarkedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public LearningPathBookMarkedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(LearningPathBookMarkedMessage message)
        {
            bool isLearningPath = message.ItemType == BookmarkType.LearningPathLMM || message.ItemType == BookmarkType.LearningPath;

            if (!isLearningPath)
            {
                return;
            }

            var existedActivity = await _dbContext
                .ActivityCollection
                .FirstOrDefaultAsync(p => p.SourceId == message.Id.ToString() && p.UserId == message.CreatedBy);

            if (existedActivity != null)
            {
                return;
            }

            var activity = new UserActivity(message.CreatedBy, message.CreatedDate, message.Id, ActivityType.BookmarkedLearningPath);

            await _dbContext.ActivityCollection.InsertOneAsync(activity);
        }
    }
}
