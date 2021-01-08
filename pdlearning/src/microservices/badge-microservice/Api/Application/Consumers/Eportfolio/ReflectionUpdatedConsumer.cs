using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Badge.Application.Consumers.Messages;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Infrastructure;
using MongoDB.Driver;

namespace Microservice.Badge.Application.Consumers
{
    [OpalConsumer("learningrecord.reflection.updated")]
    public class ReflectionUpdatedConsumer : OpalMessageConsumer<ReflectionChangedMessage>
    {
        private readonly BadgeDbContext _dbContext;

        public ReflectionUpdatedConsumer(BadgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task InternalHandleAsync(ReflectionChangedMessage message)
        {
            if (message?.EPortfolio != null)
            {
                var reflectionExisted = _dbContext
                    .ActivityCollection
                    .AsQueryable()
                    .Any(x => x.SourceId == message.Id.ToString());

                if (!reflectionExisted && (message.EPortfolio.Visibility == EPortfolioVisibility.Public || message.EPortfolio.IsExperience()))
                {
                    var activityType = message.EPortfolio.GetActivityType();
                    var reflection = new UserActivity(message.UserId, message.Reflection.Created, message.EPortfolio.Id.ToString(), activityType)
                        .WithPortfolioInfo(
                            new Domain.ValueObjects.EPortfolioInfo(
                                message.EPortfolio.Id,
                                message.EPortfolio.UserId,
                                message.EPortfolio.Visibility));

                    await _dbContext
                        .ActivityCollection
                        .InsertOneAsync(reflection);
                }
            }
        }
    }
}
