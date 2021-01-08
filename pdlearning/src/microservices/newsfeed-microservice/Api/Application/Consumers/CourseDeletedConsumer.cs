using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.NewsFeed.Application.Consumers.Messages;
using Microservice.NewsFeed.Domain.Entities;
using Microservice.NewsFeed.Infrastructure;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Microservice.NewsFeed.Application.Consumers
{
    [OpalConsumer("microservice.events.course.deleted")]
    public class CourseDeletedConsumer : OpalMessageConsumer<CourseChangeMessage>
    {
        private readonly NewsFeedDbContext _dbContext;

        public CourseDeletedConsumer(NewsFeedDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(CourseChangeMessage message)
        {
            var courseExisted = await _dbContext
                .SyncedCourseCollection
                .AsQueryable()
                .AnyAsync(_ => _.CourseId == message.Id);

            if (!courseExisted)
            {
                return;
            }

            await _dbContext
                .SyncedCourseCollection
                .DeleteOneAsync(Builders<Course>.Filter.Where(_ => _.CourseId == message.Id));
        }
    }
}
