using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Infrastructure;
using Microservice.Badge.Infrastructure.Extensions;
using MongoDB.Driver;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("csl.events.forum.deleted")]
    public class ForumDeletedConsumer : OpalMessageConsumer<ForumDeletedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public ForumDeletedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(ForumDeletedMessage message)
        {
            var forumExisted = await _dbContext
               .ActivityCollection
               .AnyAsync(x => x.SourceId == message.Id.ToString() && x.Type == ActivityType.CreateForum);

            if (!forumExisted)
            {
                return;
            }

            await _dbContext.ActivityCollection.DeleteOneAsync(x => x.SourceId == message.Id.ToString() && x.Type == ActivityType.CreateForum);
        }
    }
}
