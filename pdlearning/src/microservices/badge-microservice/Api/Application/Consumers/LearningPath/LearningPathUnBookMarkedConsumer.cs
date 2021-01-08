using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Application.Enums;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("microservice.events.learner.bookmark.deleted")]
    public class LearningPathUnBookMarkedConsumer : OpalMessageConsumer<LearningPathBookMarkedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public LearningPathUnBookMarkedConsumer(BadgeDbContext dbContext)
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

            await _dbContext.ActivityCollection.DeleteOneAsync(p => p.Id == existedActivity.Id);
        }
    }
}
